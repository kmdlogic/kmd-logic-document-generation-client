using System;
using System.Collections.Generic;
using Kmd.Logic.DocumentGeneration.Client.Models;
using Kmd.Logic.DocumentGeneration.Client.ModelTranslator;
using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.Configuration
{
    internal class DocumentGenerationConfigurationSkeleton
    {
        internal DocumentGenerationConfigurationSkeleton(
            InternalClient internalClient,
            DocumentGenerationConfigurationSummary documentGenerationConfigurationSummary,
            bool allowDeprecatedTemplateStorageTypes)
        {
            this.InternalClient = internalClient;
            this.Id = documentGenerationConfigurationSummary.Id
                      ?? throw new DocumentGenerationConfigurationException(
                          "Found null Id in DocumentGenerationConfigurationSummary");
            this.SubscriptionId = documentGenerationConfigurationSummary.SubscriptionId
                                  ?? throw new DocumentGenerationConfigurationException(
                                      "Found null SubscriptionId in DocumentGenerationConfigurationSummary");
            this.Name = documentGenerationConfigurationSummary.Name;
            this.LevelNames = new List<string>(documentGenerationConfigurationSummary.LevelNames);
            this.HasLicense = documentGenerationConfigurationSummary.HasLicense ?? false;
            this.TemplateStorageDirectory =
                documentGenerationConfigurationSummary.TemplateStorageDirectory == null
                || (!allowDeprecatedTemplateStorageTypes
                && documentGenerationConfigurationSummary.TemplateStorageDirectory.IsDeprecated())
                    ? null
                    : new DocumentGenerationTemplateStorageDirectorySkeleton(this, documentGenerationConfigurationSummary.TemplateStorageDirectory, allowDeprecatedTemplateStorageTypes);
        }

        internal Guid Id { get; set; }

        internal Guid SubscriptionId { get; set; }

        internal string Name { get; set; }

        internal bool HasLicense { get; set; }

        internal IReadOnlyList<string> LevelNames { get; set; }

        internal DocumentGenerationTemplateStorageDirectorySkeleton TemplateStorageDirectory { get; set;  }

        internal InternalClient InternalClient { get; }
    }
}
