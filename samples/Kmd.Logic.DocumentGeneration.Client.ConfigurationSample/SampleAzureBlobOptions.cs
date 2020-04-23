namespace Kmd.Logic.DocumentGeneration.Client.ConfigurationSample
{
    public class SampleAzureBlobOptions
    {
        /// <summary>
        /// Gets or sets connection string for an Azure Blob storage area.
        /// </summary>
        public string StorageConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the name of the Blob Container.
        /// </summary>
        public string ContainerName { get; set; } = @"document-generation";

        /// <summary>
        /// Gets or sets an area blob prefix.
        /// </summary>
        public string BlobPrefix { get; set; } = string.Empty;
    }
}
