// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Kmd.Logic.DocumentGeneration.Client.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class CreateConfigurationRequest
    {
        /// <summary>
        /// Initializes a new instance of the CreateConfigurationRequest class.
        /// </summary>
        public CreateConfigurationRequest()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the CreateConfigurationRequest class.
        /// </summary>
        public CreateConfigurationRequest(string name = default(string), bool? hasLicense = default(bool?), IList<string> levelNames = default(IList<string>), string metadataFilenameExtension = default(string))
        {
            Name = name;
            HasLicense = hasLicense;
            LevelNames = levelNames;
            MetadataFilenameExtension = metadataFilenameExtension;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "hasLicense")]
        public bool? HasLicense { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "levelNames")]
        public IList<string> LevelNames { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "metadataFilenameExtension")]
        public string MetadataFilenameExtension { get; set; }

    }
}
