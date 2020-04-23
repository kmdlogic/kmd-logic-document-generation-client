using System;

namespace Kmd.Logic.DocumentGeneration.Client.ServiceMessages
{
    public class DocumentGenerationConfigurationListItem
    {
        internal DocumentGenerationConfigurationListItem()
        {
        }

        public Guid ConfigurationId { get; set; }

        public string Name { get; set; }
    }
}
