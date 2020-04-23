using System;

namespace Kmd.Logic.DocumentGeneration.Client.ConfigurationSample
{
    public class ConfigurationSampleOptions
    {
        /// <summary>
        /// Gets or sets the Configuration Id of an already created document generation configuration.
        /// </summary>
        public Guid? ConfigurationId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to use SharePointOnline as the template storage.
        /// </summary>
        public bool UseSharePointOnline { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not to use Azure Blob as the template storage.
        /// </summary>
        public bool UseAzureBlob { get; set; }
    }
}
