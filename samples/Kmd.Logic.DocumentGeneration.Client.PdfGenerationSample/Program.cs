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

namespace Kmd.Logic.DocumentGeneration.Client.PdfGenerationSample
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
                        (await documentGenerationClient.GetTemplates(configurationId, hierarchyPath, subject)
                            .ConfigureAwait(false))
                        .ToDictionary(t => t.TemplateId, t => t);
                    var allFormats = Enum.GetValues(typeof(DocumentFormat)).Cast<DocumentFormat>().ToArray();

                    var documentDetailsList = new List<DocumentDetails>();

                    foreach (var templateId in new[] { BasicWordTemplateId })
                    {
                        if (!templates.TryGetValue(templateId, out var template))
                        {
                            DiagnosticLog($"Unable to find {templateId} in the template storage area");
                            continue;
                        }

                        var documentDetails = new GeneratedDocumentDetails
                        {
                            ConfigurationId = configurationId,
                            HierarchyPath = hierarchyPath,
                            Template = template,
                            MergeData = mergeData,
                            DocumentFormat = DocumentFormat.Pdf,
                            Description = $"Generated from {template.TemplateId} to {DocumentFormat.Pdf}",
                        };
                        await documentGenerationClient.GenerateDocumentFromTemplate(documentDetails)
                            .ConfigureAwait(false);
                    }
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
                    await documentGenerationClient.RequestDocumentGeneration(
                            documentDetails.ConfigurationId,
                            new DocumentGenerationRequestDetails(
                                documentDetails.HierarchyPath,
                                documentDetails.Template.TemplateId,
                                documentDetails.Template.Languages.FirstOrDefault() ?? string.Empty,
                                documentDetails.DocumentFormat,
                                documentDetails.MergeData,
                                new Uri("https://webhook.site/93b2c86b-463f-42c0-bc90-958d76043b2f"),
                                false,
                                null
                                ))
                        .ConfigureAwait(false);

                const int secondsToTimesUp = 30;
                var stopWatch = Stopwatch.StartNew();
                while (stopWatch.Elapsed.TotalSeconds < secondsToTimesUp)
                {
                    var generationProgress =
                        await documentGenerationClient.GetDocumentGenerationProgress(documentGenerationProgress.Id)
                            .ConfigureAwait(false);

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
                        await documentGenerationClient.GetDocumentGenerationUri(documentGenerationProgress.Id)
                            .ConfigureAwait(false);

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
