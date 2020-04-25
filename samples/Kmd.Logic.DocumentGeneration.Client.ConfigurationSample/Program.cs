using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kmd.Logic.DocumentGeneration.Client.Configuration;
using Kmd.Logic.DocumentGeneration.Client.Configuration.TemplateStorageConfigurations;
using Kmd.Logic.DocumentGeneration.Client.ServiceMessages;
using Kmd.Logic.DocumentGeneration.Client.Types;
using Kmd.Logic.Identity.Authorization;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;

namespace Kmd.Logic.DocumentGeneration.Client.ConfigurationSample
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
            catch (Exception ex)
            {
                Log.Fatal(ex, "Caught a fatal unhandled exception");
            }
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

                var subject = string.Empty;
                var mergeData = JObject.Parse(File.ReadAllText($"values/{CustomerDataFileName}"));
                var logicSubscriptionId = configuration.DocumentGeneration.SubscriptionId ?? throw new Exception("Subscription Id not configured");

                ITemplateStorageConfiguration sampleTemplateStorageConfiguration;
                if (configuration.ConfigurationSample.UseAzureBlob)
                {
                    sampleTemplateStorageConfiguration = new AzureBlobTemplateStorage(
                        configuration.AzureBlobSample.StorageConnectionString,
                        configuration.AzureBlobSample.ContainerName,
                        configuration.AzureBlobSample.BlobPrefix);
                }
                else
                {
                    sampleTemplateStorageConfiguration = new SharePointOnlineTemplateStorage(
                        configuration.SharePointOnlineSample.ClientId,
                        configuration.SharePointOnlineSample.TenantId,
                        configuration.SharePointOnlineSample.ClientSecret,
                        configuration.SharePointOnlineSample.GroupName);
                }

                var configurationId = configuration.ConfigurationSample.ConfigurationId ?? throw new Exception("Configuration Id not configured");

                // Get an existing document generation configuration of template storage directories
                DiagnosticLog("Getting a document generation template storage configuration.");
                var documentGenerationConfiguration =
                    documentGenerationClient.GetDocumentGenerationConfiguration(
                        logicSubscriptionId,
                        configurationId);

                // Update the existing configuration
                documentGenerationConfiguration.Name = "ConfigurationSample Name";
                documentGenerationConfiguration.LevelNames = new[] { "one", "two" };

                // Create a root directory for this configuration
                var rootTemplateStaorageDirectory =
                    documentGenerationConfiguration.SetRootTemplateStorageDirectory(
                        "Root directory",
                        sampleTemplateStorageConfiguration);

                // Create a hierarchy of sub directories.
                // In this case each sub directory is really the Logic template storage area for this subscription,
                // so they'll all resolve to the same place server side.
                foreach (var childKey in new[] { "schoolone", "schooltwo" })
                {
                    var childEntry =
                        documentGenerationConfiguration.CreateDocumentGenerationTemplateStorageDirectory(
                            childKey,
                            $"school {childKey}",
                            sampleTemplateStorageConfiguration);
                    rootTemplateStaorageDirectory.AddChild(childEntry);
                    foreach (var grandChildKey in new[] { "A", "B", "C" })
                    {
                        var grandChild =
                            documentGenerationConfiguration.CreateDocumentGenerationTemplateStorageDirectory(
                                grandChildKey,
                                $"dept {grandChildKey}",
                                sampleTemplateStorageConfiguration);
                        childEntry.AddChild(grandChild);
                    }
                }

                // Save the created configuration to the Logic server
                {
                    DiagnosticLog("Saving the rebuilt document generation template storage configuration.");
                    documentGenerationConfiguration.Save();
                    DiagnosticLog("Document generation template storage configuration uploaded.");
                }

                // For the sake of the sample, load the configuration from the sever.
                // This is not really required to proceed.
                {
                    DiagnosticLog("Downloading the saved document generation template storage configuration.");
                    documentGenerationConfiguration =
                        documentGenerationClient.GetDocumentGenerationConfiguration(
                            logicSubscriptionId,
                            documentGenerationConfiguration.Id);
                    DiagnosticLog($"Document generation template storage configuration \"{documentGenerationConfiguration.Name}\" downloaded.");
                }

                // For the sake of the sample, modify the configuration and upload again.
                // This is not required to proceed.
                {
                    DiagnosticLog("Modifying and uploading the document generation template storage configuration.");
                    var modifyThisTemplateDirectory =
                        documentGenerationConfiguration.FindDirectoryByPath(new HierarchyPath(new[] { string.Empty, "schooltwo", "B" }));
                    modifyThisTemplateDirectory.Key = "BModified";
                    documentGenerationConfiguration.Save();
                    DiagnosticLog($"Document generation template storage configuration \"{documentGenerationConfiguration.Name}\" modified and uploaded.");
                }

                // Choose an arbitrary template directory
                var hierarchyPath = new HierarchyPath(new[] { string.Empty, "schooltwo", "BModified" });
                var templateDirectory = documentGenerationConfiguration.FindDirectoryByPath(hierarchyPath);

                try
                {
                    // Get all the templates relative to that template directory and ancestor directories
                    var templates =
                        templateDirectory.GetTemplates(subject)
                            .ToDictionary(t => t.TemplateId, t => t);

                    // If the template we're interested in hasn't been uploaded to the storage area, don't continue.
                    if (!templates.TryGetValue(BasicWordTemplateId, out var template))
                    {
                        DiagnosticLog($"Unable to find {BasicWordTemplateId} in the template storage area");
                        throw new Exception("Finishing prematurely");
                    }

                    // Request a document be generated using a template found in the templateDirectory or in ancestor directories
                    DiagnosticLog($"Requesting a document be generated using template {BasicWordTemplateId} found in {hierarchyPath} or in ancestor directories");
                    var documentGenerationRequestId =
                        templateDirectory.RequestDocumentGeneration(
                            template.TemplateId,
                            template.Languages.FirstOrDefault() ?? string.Empty,
                            DocumentFormat.Docx,
                            mergeData,
                            null,
                            false)?.Id;

                    Uri downloadLink;

                    if (documentGenerationRequestId == null)
                    {
                        DiagnosticLog($"Unable to request document generation");
                        throw new Exception("Finishing prematurely");
                    }

                    try
                    {
                        downloadLink = PollForProgress(documentGenerationConfiguration, documentGenerationRequestId.Value);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Generation failed for {template.TemplateId}. Finishing prematurely", e);
                    }

                    if (downloadLink == null)
                    {
                        DiagnosticLog($"Generation took too long. Finishing prematurely");
                        return;
                    }

                    DiagnosticLog($"{template.TemplateId}: secure download link found: {downloadLink}");
                    var filename = GetTempFilename(template.TemplateId, DocumentFormat.Docx.ToString().ToLower());
                    await DownloadToFilesystem(downloadLink, filename, $"Generated {template.TemplateId}").ConfigureAwait(false);

                    // Request the new document be converted to Pdf/A
                    DiagnosticLog($"Requesting the generated document be converted to Pdf/A");
                    var converstionRequestId =
                        documentGenerationConfiguration.RequestDocumentConversionToPdfA(
                            new DocumentConversionToPdfARequestDetails(
                                downloadLink,
                                DocumentFormat.Docx,
                                null,
                                false))?.Id;

                    Uri convertedDocumentDownloadLink;

                    if (converstionRequestId == null)
                    {
                        DiagnosticLog($"Unable to request document conversion");
                        throw new Exception("Finishing prematurely");
                    }
                    else
                    {
                        try
                        {
                            convertedDocumentDownloadLink = PollForProgress(documentGenerationConfiguration, converstionRequestId.Value);
                        }
                        catch (Exception e)
                        {
                            throw new Exception($"Conversion to Pdf/A failed for {template.TemplateId}. Finishing prematurely", e);
                        }
                    }

                    if (convertedDocumentDownloadLink == null)
                    {
                        DiagnosticLog($"Conversion took too long. Finishing prematurely");
                        return;
                    }

                    DiagnosticLog($"{template.TemplateId}: secure download link to rendered document found: {convertedDocumentDownloadLink}");
                    var convertedDocumentFilename = GetTempFilename(template.TemplateId, DocumentFormat.Pdf.ToString().ToLower());
                    await DownloadToFilesystem(convertedDocumentDownloadLink, convertedDocumentFilename, $"Rendered {template.TemplateId}").ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    DiagnosticLog($"{e.Message}/{e.StackTrace}");
                }
            }
        }

        // poll for the status of the document preparation process
        private static Uri PollForProgress(
            DocumentGenerationConfiguration documentGenerationConfiguration,
            Guid documentGenerationRequestId)
        {
            const int secondsToTimesUp = 30;
            var stopWatch = Stopwatch.StartNew();
            while (stopWatch.Elapsed.TotalSeconds < secondsToTimesUp)
            {
                var documentGenerationProgress =
                    documentGenerationConfiguration.GetDocumentGenerationProgress(documentGenerationRequestId);
                switch (documentGenerationProgress.State)
                {
                    case DocumentGenerationState.Failed:
                        throw new Exception($"Document preparattion failed for template: {documentGenerationProgress.FailReason}");
                    case DocumentGenerationState.Requested:
                        DiagnosticLog($"Document not yet ready.");
                        Thread.Sleep(300);
                        continue;
                    case DocumentGenerationState.Completed:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                var documentGenerationUri =
                    documentGenerationConfiguration.GetDocumentGenerationUri(documentGenerationRequestId);

                if (documentGenerationUri?.Uri != null)
                {
                    return documentGenerationUri.Uri;
                }

                DiagnosticLog($"Document preparation polling gave up after {secondsToTimesUp} seconds.");
            }

            return null;
        }

        private static async Task DownloadToFilesystem(Uri downloadLink, string filename, string description)
        {
            await using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var response = await new HttpClient().GetAsync(downloadLink).ConfigureAwait(false);
                await response.Content.CopyToAsync(fs).ConfigureAwait(false);
            }

            DiagnosticLog($"{description}: Prepared document written to {filename}");
        }

        private static void DiagnosticLog(string message)
        {
            Log.Write(LogEventLevel.Information, $"{DateTime.Now:o}: {message}");
        }

        private static string GetTempFilename(string sample, string ext)
        {
            var filenameStem = Path.GetFileNameWithoutExtension(sample);
            return Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid().ToString()}_from_{filenameStem}.{ext}");
        }
    }
}
