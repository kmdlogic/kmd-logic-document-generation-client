using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kmd.Logic.DocumentGeneration.Client.Models;
using Kmd.Logic.Identity.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Serilog;

namespace Kmd.Logic.DocumentGeneration.Client.Sample
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

                var configurationId = Guid.NewGuid();
                var hierarchyPath = @"mycust\mydept";
                var subject = "testsubject";
                var defaultLanguageName = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                var mergeData = JObject.Parse(File.ReadAllText($"values/{CustomerDataFileName}"));

                try
                {
                    var templates =
                        (await documentGenerationClient.GetTemplatesAsync(null, configurationId, hierarchyPath, subject)
                            .ConfigureAwait(false))
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
                            var documentDetails = new DocumentDetails
                            {
                                ConfigurationId = configurationId,
                                HierarchyPath = hierarchyPath,
                                Template = template,
                                MergeData = mergeData,
                                DocumentFormat = format,
                            };
                            await documentGenerationClient.GenerateDocumentFromTemplate(documentDetails)
                                .ConfigureAwait(false);
                            if (documentDetails.AnchorDetails != null)
                            {
                                documentDetailsList.Add(documentDetails);
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

        private static async Task GenerateDocumentFromTemplate(this DocumentGenerationClient documentGenerationClient, DocumentDetails documentDetails)
        {
            try
            {
                DiagnosticLog($"Requesting document generation from template {documentDetails.Template.TemplateId}");
                var documentGenerationRequest =
                    await documentGenerationClient.RequestDocumentGenerationAsync(null, new GenerateDocumentRequest
                        {
                            ConfigurationId = documentDetails.ConfigurationId,
                            HierarchyPath = documentDetails.HierarchyPath,
                            TemplateId = documentDetails.Template.TemplateId,
                            Language = documentDetails.Template.Languages.FirstOrDefault() ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName,
                            DocumentFormat = documentDetails.DocumentFormat.ToString(),
                            MergeData = documentDetails.MergeData,
                        })
                        .ConfigureAwait(false);
                if (documentGenerationRequest.Id != null)
                {
                    const int secondsToTimesUp = 10;
                    var stopWatch = Stopwatch.StartNew();
                    while (stopWatch.Elapsed.TotalSeconds < secondsToTimesUp)
                    {
                        var documentGenerationRequestStatusCheck =
                            await documentGenerationClient
                                .GetDocumentGenerationAsync(null, documentGenerationRequest.Id)
                                .ConfigureAwait(false);
                        var documentGenerationState =
                            Enum.Parse(typeof(DocumentGenerationState), documentGenerationRequestStatusCheck.State);
                        switch (documentGenerationState)
                        {
                            case DocumentGenerationState.Failed:
                                DiagnosticLog(
                                    $"Document generation failed for template: {documentDetails.Template.TemplateId}");
                                throw new Exception("Document generation failed for template: ");
                            case DocumentGenerationState.Requested:
                                DiagnosticLog($"{documentDetails.Template.TemplateId}: document not yet ready.");
                                Thread.Sleep(300);
                                continue;
                            default:
                                break;
                        }

                        var filename = GetTempFilename(documentDetails.Template, documentDetails.DocumentFormat.ToString().ToLower());
                        await using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            await documentGenerationClient.WriteDocumentToStreamAsync(null, documentGenerationRequest.Id.Value, fs);
                        }

                        DiagnosticLog($"{documentDetails.Template.TemplateId}: Generated document written to {filename}");
                        documentDetails.LocalDownloadPath = filename;

                        var documentUri = await documentGenerationClient
                            .GetDocumentAsync(null, documentGenerationRequest.Id.Value)
                            .ConfigureAwait(false);

                        if (documentUri?.Uri != null)
                        {
                            documentDetails.AnchorDetails = new AnchorDetails
                            {
                                Href = documentUri.Uri,
                                Expiry = documentUri.UriExpiryTime ?? DateTime.Now,
                                LinkText = $"Generated Doc.{documentDetails.DocumentFormat.ToString().ToLower()}",
                            };
                        }

                        return;
                    }

                    DiagnosticLog($"{documentDetails.Template.TemplateId}: Gave up after {secondsToTimesUp} seconds.");
                }
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
                docLinksFw.WriteLine("<th>Template</th>");
                docLinksFw.WriteLine("<th>Link Expiry</th>");
                docLinksFw.WriteLine("<th>Link</th>");
                docLinksFw.WriteLine("</tr>");
                foreach (var documentDetails in documentDetailsList)
                {
                    docLinksFw.WriteLine("<tr>");
                    docLinksFw.WriteLine($"<td>{documentDetails.Template.TemplateId}</td>");
                    docLinksFw.WriteLine($"<td>{documentDetails.AnchorDetails.Expiry.ToString("s")}</td>");
                    docLinksFw.WriteLine($"<td><a target=\"_self\" href=\"{documentDetails.AnchorDetails.Href}\">{documentDetails.AnchorDetails.LinkText}</a></td>");
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

        private static string GetTempFilename(Template template, string ext)
        {
            var filenameStem = Path.GetFileNameWithoutExtension(template.TemplateId);
            return Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString()}_from_{filenameStem}.{ext}");
        }
    }
}
