using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
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
                var configurationKey = Guid.NewGuid();
                var templateId = "TestTemplate.docx";
                var language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
                var mergeData = JObject.Parse(@"{
                    'Name': 'John Doe',
                    'CourseName': 'Master Builder',
                    'CompletionDate': '2020-06-31',
                    'PrincipalName': 'John Hancock',
                    'PrincipalTitle': 'Principal NEXT College',
                    'Results': [
                        {
                            'Name': 'Advanced Carpentry',
                            'Grade': 'A'
                        },
                        {
                            'Name': 'Intermediate Welding',
                            'Grade': 'B-'
                        },
                        {
                            'Name': 'Quotations and Invoicing',
                            'Grade': 'A+'
                        },
                        {
                            'Name': 'Subcontracting',
                            'Grade': 'B'
                        },
                        {
                            'Name': 'Advanced Bricklaying',
                            'Grade': 'B+'
                        },
                        {
                            'Name': 'Intermediate Rendering',
                            'Grade': 'D'
                        }
                    ]
                }");

                try
                {
                    var documentGenerationRequest =
                        await documentGenerationClient.RequestDocumentGenerationAsync(null, new GenerateDocumentRequest
                            {
                                ConfigurationId = configurationId,
                                Key = configurationKey,
                                TemplateId = templateId,
                                Language = language,
                                MergeData = mergeData,
                            })
                            .ConfigureAwait(false);
                    var documentGenerationRequestReadBack =
                        await documentGenerationClient.GetDocumentGenerationAsync(null, documentGenerationRequest.Id)
                            .ConfigureAwait(false);
                    if (documentGenerationRequestReadBack == null
                        || documentGenerationRequestReadBack.Id != documentGenerationRequest.Id
                        || documentGenerationRequestReadBack.Key != documentGenerationRequest.Key
                        || documentGenerationRequestReadBack.TemplateId != documentGenerationRequest.TemplateId
                        || documentGenerationRequestReadBack.Language != documentGenerationRequest.Language)
                    {
                        Console.Error.WriteLine($"RequestDocumentGeneration details and GetDocumentGenerationDetails differ");
                    }

                    if (documentGenerationRequest.Id != null)
                    {
                        var documentStream =
                            await documentGenerationClient.GetDocumentAsync(null, documentGenerationRequest.Id.Value)
                                .ConfigureAwait(false);
                        if (documentStream != null)
                        {
                            var filename = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                            await using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
                            {
                                await documentStream.CopyToAsync(fs).ConfigureAwait(false);
                                fs.Close();
                            }

                            Console.WriteLine($"Generated document written to {filename}");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"{e.Message}/{e.StackTrace}");
                }
            }
        }
    }
}
