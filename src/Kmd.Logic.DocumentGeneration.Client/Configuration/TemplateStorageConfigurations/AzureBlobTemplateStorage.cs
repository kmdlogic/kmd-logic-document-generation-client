using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.Configuration.TemplateStorageConfigurations
{
    public class AzureBlobTemplateStorage : ITemplateStorageConfiguration
    {
        public TemplateStorageType TemplateStorageType => TemplateStorageType.AzureBlobStorage;

        public string SecretKeyOrStorageConnectionString { get; set; }

        public string ContainerName { get; set; }

        public string BlobPrefix { get; set; }

        public AzureBlobTemplateStorage(
            string secretKeyOrStorageConnectionString,
            string containerName,
            string blobPrefix)
        {
            this.SecretKeyOrStorageConnectionString = secretKeyOrStorageConnectionString;
            this.ContainerName = containerName;
            this.BlobPrefix = blobPrefix;
        }

        internal AzureBlobTemplateStorage(Models.AzureBlobStorageConfiguration azureBlobStorageConfiguration)
        {
            this.SecretKeyOrStorageConnectionString = azureBlobStorageConfiguration.SecretKeyForStorageConnectionString;
            this.ContainerName = azureBlobStorageConfiguration.ContainerName;
            this.BlobPrefix = azureBlobStorageConfiguration.BlobPrefix;
        }
    }
}
