using System;
using System.Collections.Generic;
using Kmd.Logic.DocumentGeneration.Client.Configuration.TemplateStorageConfigurations;
using Kmd.Logic.DocumentGeneration.Client.Models;
using Kmd.Logic.DocumentGeneration.Client.ModelTranslator;
using Kmd.Logic.DocumentGeneration.Client.ServiceMessages;
using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.Configuration
{
    public class DocumentGenerationConfiguration
    {
        internal InternalClient InternalClient { get; }

        public Guid Id { get; private set; }

        public Guid SubscriptionId { get; private set; }

        public string Name { get; set; }

        private DocumentGenerationTemplateStorageDirectory _templateStorageDirectory;

        public DocumentGenerationTemplateStorageDirectory TemplateStorageDirectory
        {
            get => this._templateStorageDirectory;

            private set
            {
                value?.UpdateProperties(string.Empty, value.Name, value.TemplateStorageConfiguration);
                this._templateStorageDirectory = value;
            }
        }

        public IList<string> LevelNames { get; set; }

        public bool HasLicense { get; set; }

        internal DocumentGenerationConfiguration(InternalClient internalClient, Guid subscriptionId, Guid configurationId)
        {
            this.InternalClient = internalClient;
            this.SubscriptionId = subscriptionId;
            this.Id = configurationId;
            this.Load();
        }

        internal static DocumentGenerationConfiguration Create(
            InternalClient internalClient,
            Guid subscriptionId,
            string name,
            string[] levelNames,
            bool hasLicense)
        {
            return new DocumentGenerationConfiguration(internalClient, subscriptionId, name, levelNames, hasLicense);
        }

        private DocumentGenerationConfiguration(
            InternalClient internalClient,
            Guid subscriptionId,
            string name,
            string[] levelNames,
            bool hasLicense)
        {
            this.InternalClient = internalClient;
            this.SubscriptionId = subscriptionId;
            this.Name = name;
            this.LevelNames = levelNames;
            this.HasLicense = hasLicense;
        }

        public void Save()
        {
            if (this.TemplateStorageDirectory == null)
            {
                throw new DocumentGenerationConfigurationException("Cannot Save without a TemplateStorageDirectory");
            }

            DocumentGenerationConfigurationSummary serverDocumentGenerationConfigurationSummary = null;
            if (this.Id != Guid.Empty)
            {
                serverDocumentGenerationConfigurationSummary =
                    this.InternalClient
                        .GetConfigurationSummaryByIdWithHttpMessagesAsync(this.SubscriptionId, this.Id)
                        .ValidateBody();
            }

            if (serverDocumentGenerationConfigurationSummary == null)
            {
                throw new DocumentGenerationConfigurationException("Cannot Save a Configuration unless it has already been created on the server.");
            }

            var serverDocumentGenerationConfigurationSkeleton = new DocumentGenerationConfigurationSkeleton(this.InternalClient, serverDocumentGenerationConfigurationSummary, true);
            this.Id = serverDocumentGenerationConfigurationSkeleton.Id;
            this.InternalClient.UpdateDocumentGenerationConfigurationWithHttpMessagesAsync(
                    this.SubscriptionId,
                    this.Id,
                    new UpdateConfigurationRequest(
                        this.Name,
                        this.HasLicense,
                        this.LevelNames))
                .ValidateBody();
            this.TemplateStorageDirectory.Save(serverDocumentGenerationConfigurationSkeleton.TemplateStorageDirectory);
        }

        public void Load()
        {
            var serverDocumentGenerationConfigurationSummary =
                this.InternalClient
                    .GetConfigurationSummaryByIdWithHttpMessagesAsync(this.SubscriptionId, this.Id)
                    .ValidateBody();

            var documentGenerationConfigurationSkeleton = new DocumentGenerationConfigurationSkeleton(this.InternalClient, serverDocumentGenerationConfigurationSummary, false);
            this.UpdateProperties(documentGenerationConfigurationSkeleton);
            var documentGenerationConfigurationEntrySkeleton = documentGenerationConfigurationSkeleton.TemplateStorageDirectory;
            if (documentGenerationConfigurationEntrySkeleton != null)
            {
                var entryId = documentGenerationConfigurationEntrySkeleton.Id;
                if (this.TemplateStorageDirectory != null && this.TemplateStorageDirectory.Id == entryId)
                {
                    this.TemplateStorageDirectory.Load(documentGenerationConfigurationEntrySkeleton);
                }
                else
                {
                    this.TemplateStorageDirectory = new DocumentGenerationTemplateStorageDirectory(this, null, documentGenerationConfigurationEntrySkeleton);
                }
            }
            else
            {
                this.TemplateStorageDirectory = null;
            }
        }

        public void Delete()
        {
            this.InternalClient.DeleteDocumentGenerationConfiguration(this.SubscriptionId, this.Id);
        }

        public DocumentGenerationTemplateStorageDirectory SetRootTemplateStorageDirectory(string name, ITemplateStorageConfiguration templateStorageConfiguration)
        {
            this.TemplateStorageDirectory =
                this.CreateDocumentGenerationTemplateStorageDirectory(string.Empty, name, templateStorageConfiguration);
            return this.TemplateStorageDirectory;
        }

        public DocumentGenerationTemplateStorageDirectory CreateDocumentGenerationTemplateStorageDirectory(
            string key,
            string name,
            ITemplateStorageConfiguration templateStorageConfiguration)
        {
            return DocumentGenerationTemplateStorageDirectory.Create(this, key, name, templateStorageConfiguration);
        }

        public DocumentGenerationTemplateStorageDirectory FindDirectoryByPath(HierarchyPath path)
        {
            return this.TemplateStorageDirectory?.FindDirectoryByPath(path);
        }

        public DocumentGenerationProgress GetDocumentGenerationProgress(Guid documentGenerationRequestId)
        {
            var documentGenerationRequest =
                this.InternalClient
                    .GetDocumentGenerationWithHttpMessagesAsync(this.SubscriptionId, documentGenerationRequestId)
                    .ValidateBody();
            return documentGenerationRequest.ToDocumentGenerationProgress();
        }

        public DocumentGenerationUri GetDocumentGenerationUri(Guid documentGenerationRequestId)
        {
            var documentUri =
                this.InternalClient
                    .GetDocumentWithHttpMessagesAsync(this.SubscriptionId, documentGenerationRequestId)
                    .ValidateBody();
            return documentUri?.ToDocumentGenerationUri();
        }

        public DocumentGenerationProgress RequestDocumentConversionToPdfA(DocumentConversionToPdfARequestDetails documentConversionToPdfARequestDetails)
        {
            var documentGenerationRequest =
                this.InternalClient.RequestDocumentConversionWithHttpMessagesAsync(this.SubscriptionId, documentConversionToPdfARequestDetails.ToWebRequest(this.Id))
                    .ValidateBody();
            return documentGenerationRequest.ToDocumentGenerationProgress();
        }

        public DocumentGenerationProgress RequestDocumentConversion(DocumentConversionRequestDetails documentConversionRequestDetails)
        {
            var documentGenerationRequest =
                this.InternalClient.RequestDocumentConversionWithHttpMessagesAsync(this.SubscriptionId, documentConversionRequestDetails.ToWebRequest(this.Id))
                    .ValidateBody();
            return documentGenerationRequest.ToDocumentGenerationProgress();
        }

        private void UpdateProperties(DocumentGenerationConfigurationSkeleton documentGenerationConfigurationSummary)
        {
            this.Id = documentGenerationConfigurationSummary.Id;
            this.SubscriptionId = documentGenerationConfigurationSummary.SubscriptionId;
            this.Name = documentGenerationConfigurationSummary.Name;
            this.LevelNames = new List<string>(documentGenerationConfigurationSummary.LevelNames);
            this.HasLicense = documentGenerationConfigurationSummary.HasLicense;
        }
    }
}
