using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kmd.Logic.DocumentGeneration.Client.ServiceMessages;
using Kmd.Logic.DocumentGeneration.Client.Types;
using Kmd.Logic.Identity.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Kmd.Logic.DocumentGeneration.Client.GenerationSample
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            InitLogger();

            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddUserSecrets(typeof(Program).Assembly)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build()
                    .Get<AppConfiguration>();

                await Run(config).ConfigureAwait(false);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
            {
                Log.Fatal(ex, "Caught a fatal unhandled exception");
            }
#pragma warning restore CA1031 // Do not catch general exception types
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void InitLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();
        }

        private const string BasicWordTemplateId = "word-template.docx";
        private const string WordWithPartialTemplateId = "word-template-with-loading-partial.docx";
        private const string WordToSaveAsTxtTemplateId = "word-template-to-txt.docx";
        private const string CustomerDataFileName = "customer.json";

        private static async Task Run(AppConfiguration configuration)
        {
            var validator = new ConfigurationValidator(configuration);
            if (!validator.Validate())
            {
                return;
            }

            using (var httpClient = new HttpClient())
            using (var tokenProviderFactory = new LogicTokenProviderFactory(configuration.TokenProvider))
            {
                var documentGenerationClient = new DocumentGenerationClient(httpClient, tokenProviderFactory, configuration.DocumentGeneration);

                var configurationId = configuration.GenerationSample.ConfigurationId ?? throw new Exception("Validation should have reported no ConfigurationId set");
                var hierarchyPath = configuration.GenerationSample.HierarchyPath;
                var subject = configuration.GenerationSample.Subject;
                var mergeData = JObject.Parse(File.ReadAllText($"values/{CustomerDataFileName}"));

                try
                {
                    var templates =
                        documentGenerationClient.GetTemplates(null, configurationId, hierarchyPath, subject)
                        .ToDictionary(t => t.TemplateId, t => t);
                    var allFormats = Enum.GetValues(typeof(DocumentFormat)).Cast<DocumentFormat>().ToArray();

                    var documentDetailsList = new List<DocumentDetails>();

                    foreach (var templateId in new[] { BasicWordTemplateId, WordWithPartialTemplateId, WordToSaveAsTxtTemplateId })
                    {
                        if (!templates.TryGetValue(templateId, out var template))
                        {
                            DiagnosticLog($"Unable to find {templateId} in the template storage area");
                            continue;
                        }

                        foreach (var format in allFormats)
                        {
                            var documentDetails = new GeneratedDocumentDetails
                            {
                                ConfigurationId = configurationId,
                                HierarchyPath = hierarchyPath,
                                Template = template,
                                MergeData = mergeData,
                                DocumentFormat = format,
                                Description = $"Generated from {template.TemplateId} to {format}",
                            };
                            await documentGenerationClient.GenerateDocumentFromTemplate(documentDetails)
                                .ConfigureAwait(false);
                            if (documentDetails.AnchorDetails != null)
                            {
                                documentDetailsList.Add(documentDetails);
                                if (documentDetails.AnchorDetails.Href != null)
                                {
                                    var convertedDocumentDetails = new ConvertedDocumentDetails
                                    {
                                        ConfigurationId = configurationId,
                                        SourceDocumentUrl = documentDetails.AnchorDetails.Href,
                                        SourceDocumentFormat = format,
                                        DocumentFormat = DocumentFormat.Pdf,
                                        Description = $"Rendered from ({documentDetails.Description})",
                                    };
                                    await documentGenerationClient.RenderPdfAFromDocumentLink(convertedDocumentDetails)
                                        .ConfigureAwait(false);
                                    if (convertedDocumentDetails.AnchorDetails != null)
                                    {
                                        documentDetailsList.Add(convertedDocumentDetails);
                                    }
                                }
                            }
                        }
                    }

                    GeneratePageWithDocumentLinks(documentDetailsList);
                }
                catch (Exception e)
                {
                    DiagnosticLog($"{e.Message}/{e.StackTrace}");
                }
            }
        }

        private static async Task GenerateDocumentFromTemplate(this DocumentGenerationClient documentGenerationClient, GeneratedDocumentDetails documentDetails)
        {
            try
            {
                DiagnosticLog($"Requesting document generation from template {documentDetails.Template.TemplateId}");
                var documentGenerationProgress =
                    documentGenerationClient.RequestDocumentGeneration(
                        null,
                        documentDetails.ConfigurationId,
                        new DocumentGenerationRequestDetails(
                            documentDetails.HierarchyPath,
                            documentDetails.Template.TemplateId,
                            documentDetails.Template.Languages.FirstOrDefault() ?? string.Empty,
                            documentDetails.DocumentFormat,
                            documentDetails.MergeData,
                            null,
                            false));

                const int secondsToTimesUp = 30;
                var stopWatch = Stopwatch.StartNew();
                while (stopWatch.Elapsed.TotalSeconds < secondsToTimesUp)
                {
                    var generationProgress =
                        documentGenerationClient.GetDocumentGenerationProgress(null, documentGenerationProgress.Id);

                    switch (generationProgress.State)
                    {
                        case DocumentGenerationState.Failed:
                            DiagnosticLog(
                                $"Document generation failed for template: {documentDetails.Template.TemplateId}: {generationProgress.FailReason}");
                            throw new Exception("Document generation failed for template: ");
                        case DocumentGenerationState.Requested:
                            DiagnosticLog($"{documentDetails.Template.TemplateId}: document not yet ready.");
                            Thread.Sleep(300);
                            continue;
                        case DocumentGenerationState.Completed:
                            break;
                    }

                    var documentUri =
                        documentGenerationClient.GetDocumentGenerationUri(null, documentGenerationProgress.Id);

                    if (documentUri?.Uri != null)
                    {
                        var filename =
                            GetTempFilename(documentDetails.Template.TemplateId, documentDetails.DocumentFormat.Ext());
                        await using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            var response = await new HttpClient().GetAsync(documentUri.Uri).ConfigureAwait(false);
                            await response.Content.CopyToAsync(fs).ConfigureAwait(false);
                        }

                        DiagnosticLog(
                            $"{documentDetails.Template.TemplateId}: Generated document written to {filename}");
                        documentDetails.LocalDownloadPath = filename;
                        documentDetails.FileSize = new FileInfo(filename).Length;

                        documentDetails.AnchorDetails = new AnchorDetails
                        {
                            Href = documentUri.Uri,
                            Expiry = documentUri.UriExpiryTime,
                            LinkText = $"download.{documentDetails.DocumentFormat.Ext()}",
                        };
                    }

                    return;
                }

                DiagnosticLog($"{documentDetails.Template.TemplateId}: Gave up after {secondsToTimesUp} seconds.");
            }
            catch (Exception e)
            {
                DiagnosticLog($"{e.Message}/{e.StackTrace}");
            }
        }

        private static async Task RenderPdfAFromDocumentLink(this DocumentGenerationClient documentGenerationClient, ConvertedDocumentDetails convertedDocumentDetails)
        {
            var ext = convertedDocumentDetails.DocumentFormat;

            try
            {
                DiagnosticLog($"Requesting document conversion from url {convertedDocumentDetails.Description}");
                var documentGenerationProgress =
                    documentGenerationClient.RequestDocumentConversionToPdfA(
                        null,
                        convertedDocumentDetails.ConfigurationId,
                        new DocumentConversionToPdfARequestDetails(
                            convertedDocumentDetails.SourceDocumentUrl,
                            convertedDocumentDetails.SourceDocumentFormat));

                const int secondsToTimesUp = 30;
                var stopWatch = Stopwatch.StartNew();
                while (stopWatch.Elapsed.TotalSeconds < secondsToTimesUp)
                {
                    var documentGenerationRequestStatusCheck =
                        documentGenerationClient.GetDocumentGenerationProgress(null, documentGenerationProgress.Id);

                    switch (documentGenerationRequestStatusCheck.State)
                    {
                        case DocumentGenerationState.Failed:
                            DiagnosticLog(
                                $"Document conversion failed for {convertedDocumentDetails.Description}: {documentGenerationRequestStatusCheck.FailReason}");
                            throw new Exception("Document generation failed for template: ");
                        case DocumentGenerationState.Requested:
                            DiagnosticLog($"{convertedDocumentDetails.Description}: document not yet ready.");
                            Thread.Sleep(300);
                            continue;
                        case DocumentGenerationState.Completed:
                            break;
                    }

                    var documentUri =
                        documentGenerationClient.GetDocumentGenerationUri(null, documentGenerationProgress.Id);

                    if (documentUri?.Uri != null)
                    {
                        var filename =
                            GetTempFilename(convertedDocumentDetails.Description, ext.Ext());
                        await using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            var response = await new HttpClient().GetAsync(documentUri.Uri).ConfigureAwait(false);
                            await response.Content.CopyToAsync(fs).ConfigureAwait(false);
                        }

                        DiagnosticLog(
                            $"{convertedDocumentDetails.Description}: Rendered document written to {filename}");
                        convertedDocumentDetails.LocalDownloadPath = filename;
                        convertedDocumentDetails.FileSize = new FileInfo(filename).Length;

                        convertedDocumentDetails.AnchorDetails = new AnchorDetails
                        {
                            Href = documentUri.Uri,
                            Expiry = documentUri.UriExpiryTime,
                            LinkText = $"download.{ext.Ext()}",
                        };
                    }

                    return;
                }

                DiagnosticLog($"{convertedDocumentDetails.Description}: Gave up after {secondsToTimesUp} seconds.");
            }
            catch (Exception e)
            {
                DiagnosticLog($"{e.Message}/{e.StackTrace}");
            }
        }

        private static void GeneratePageWithDocumentLinks(List<DocumentDetails> documentDetailsList)
        {
            var filename = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString()}_alllinks.html");
            using (var docLinksFw = new StreamWriter(filename))
            {
                docLinksFw.WriteLine("<html>");
                docLinksFw.WriteLine("<head></head>");
                docLinksFw.WriteLine("<body>");
                docLinksFw.WriteLine("<table cellpadding=\"10\">");
                docLinksFw.WriteLine("<tr>");
                docLinksFw.WriteLine("<th>Process</th>");
                docLinksFw.WriteLine("<th>Link Expiry</th>");
                docLinksFw.WriteLine("<th>Link</th>");
                docLinksFw.WriteLine("<th>Size (bytes)</th>");
                docLinksFw.WriteLine("</tr>");
                foreach (var documentDetails in documentDetailsList)
                {
                    docLinksFw.WriteLine("<tr>");
                    docLinksFw.WriteLine($"<td>{documentDetails.Description}</td>");
                    docLinksFw.WriteLine($"<td>{documentDetails.AnchorDetails.Expiry.ToString("s")}</td>");
                    docLinksFw.WriteLine($"<td><a target=\"_self\" href=\"{documentDetails.AnchorDetails.Href}\">{documentDetails.AnchorDetails.LinkText}</a></td>");
                    docLinksFw.WriteLine($"<td align=\"right\">{documentDetails.FileSize}</td>");
                    docLinksFw.WriteLine("</tr>");
                }

                docLinksFw.WriteLine("</table>");
                docLinksFw.WriteLine("</body>");
                docLinksFw.WriteLine("</html>");
            }

            DiagnosticLog($"Generated document of links written to {filename}");
        }

        private static void DiagnosticLog(string message)
        {
            Console.Out.WriteLine($"{DateTime.Now:o}: {message}");
        }

        private static string GetTempFilename(string description, string ext)
        {
            return Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString()}__{description.ReplaceExtension(ext)}");
        }

        private static string ReplaceExtension(this string filename, string ext)
        {
            var filenameStem = Path.GetFileNameWithoutExtension(filename);
            return $"{filenameStem}.{ext}";
        }

        private static string Ext(this DocumentFormat documentFormat)
        {
            return documentFormat.ToString().ToLower();
        }
    }
}
