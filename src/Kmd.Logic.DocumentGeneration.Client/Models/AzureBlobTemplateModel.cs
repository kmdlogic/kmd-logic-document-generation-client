// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Kmd.Logic.DocumentGeneration.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class AzureBlobTemplateModel
    {
        /// <summary>
        /// Initializes a new instance of the AzureBlobTemplateModel class.
        /// </summary>
        public AzureBlobTemplateModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the AzureBlobTemplateModel class.
        /// </summary>
        public AzureBlobTemplateModel(string secretKeyOrStorageConnectionString = default(string), string containerName = default(string), string blobPrefix = default(string))
        {
            SecretKeyOrStorageConnectionString = secretKeyOrStorageConnectionString;
            ContainerName = containerName;
            BlobPrefix = blobPrefix;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "secretKeyOrStorageConnectionString")]
        public string SecretKeyOrStorageConnectionString { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "containerName")]
        public string ContainerName { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "blobPrefix")]
        public string BlobPrefix { get; set; }

    }
}
