using System;
using Kmd.Logic.DocumentGeneration.Client.Configuration.TemplateStorageConfigurations;

namespace Kmd.Logic.DocumentGeneration.Client.Configuration
{
    internal class DocumentGenerationTemplateStorageDirectoryDetails
    {
        internal Guid Id { get; set; }

        internal string Key { get; set; }

        internal string Name { get; set; }

        internal ITemplateStorageConfiguration TemplateStorageConfiguration { get; set; }
    }
}
