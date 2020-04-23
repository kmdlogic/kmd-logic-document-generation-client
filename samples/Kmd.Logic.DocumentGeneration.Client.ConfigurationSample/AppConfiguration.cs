using Kmd.Logic.Identity.Authorization;

namespace Kmd.Logic.DocumentGeneration.Client.ConfigurationSample
{
    internal class AppConfiguration
    {
        public LogicTokenProviderOptions TokenProvider { get; set; } = new LogicTokenProviderOptions();

        public DocumentGenerationOptions DocumentGeneration { get; set; } = new DocumentGenerationOptions();

        public ConfigurationSampleOptions ConfigurationSample { get; set; } = new ConfigurationSampleOptions();

        public SampleAzureBlobOptions AzureBlobSample { get; set; } = new SampleAzureBlobOptions();

        public SampleSharePointOnlineOptions SharePointOnlineSample { get; set; } = new SampleSharePointOnlineOptions();
    }
}
