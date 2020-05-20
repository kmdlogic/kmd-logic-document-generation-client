using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kmd.Logic.DocumentGeneration.Client.Models;
using Kmd.Logic.DocumentGeneration.Client.ModelTranslator;
using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.Configuration
{
    internal class DocumentGenerationTemplateStorageDirectorySkeleton
    {
        internal DocumentGenerationTemplateStorageDirectorySkeleton(
            DocumentGenerationConfigurationSkeleton documentGenerationConfigurationSkeleton,
            DocumentGenerationConfigurationEntrySummary documentGenerationConfigurationEntrySummary,
            bool allowDeprecatedTemplateStorageTypes)
        {
            this.Id = documentGenerationConfigurationEntrySummary.Id ?? throw new DocumentGenerationConfigurationException("Found null Id in DocumentGenerationConfigurationEntrySummary");
            this.Key = documentGenerationConfigurationEntrySummary.Key;
            this.Name = documentGenerationConfigurationEntrySummary.Name;
            this.TemplateStorageType = documentGenerationConfigurationEntrySummary.TemplateStorageType.ToTemplateStorageType();
            this._children =
                documentGenerationConfigurationEntrySummary.Children?
                    .Where(c =>
                        allowDeprecatedTemplateStorageTypes || !c.IsDeprecated())
                    .Select(c =>
                        new DocumentGenerationTemplateStorageDirectorySkeleton(documentGenerationConfigurationSkeleton, c, allowDeprecatedTemplateStorageTypes))
                    .ToList();
            this.DocumentGenerationConfigurationSkeleton = documentGenerationConfigurationSkeleton;
        }

        internal Guid Id { get; set; }

        internal string Key { get; set; }

        internal string Name { get; set; }

        internal TemplateStorageType TemplateStorageType { get; set; }

        private List<DocumentGenerationTemplateStorageDirectorySkeleton> _children;

        internal IReadOnlyList<DocumentGenerationTemplateStorageDirectorySkeleton> Children
        {
            get => this._children.AsReadOnly();
            set => this._children = new List<DocumentGenerationTemplateStorageDirectorySkeleton>(value ?? Enumerable.Empty<DocumentGenerationTemplateStorageDirectorySkeleton>());
        }

        internal Guid SubscriptionId => this.DocumentGenerationConfigurationSkeleton.SubscriptionId;

        internal Guid ConfigurationId => this.DocumentGenerationConfigurationSkeleton.Id;

        private DocumentGenerationConfigurationSkeleton DocumentGenerationConfigurationSkeleton { get; }

        internal InternalClient InternalClient => this.DocumentGenerationConfigurationSkeleton.InternalClient;

        internal Task DeleteOnServer()
        {
            return this.DeleteEntryOnServer();
        }
    }
}
