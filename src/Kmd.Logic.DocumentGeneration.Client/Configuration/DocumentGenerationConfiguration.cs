﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public string MetadataFilenameExtension { get; set; }

        public bool HasLicense { get; set; }

        internal DocumentGenerationConfiguration(InternalClient internalClient, Guid subscriptionId, Guid configurationId)
        {
            this.InternalClient = internalClient;
            this.SubscriptionId = subscriptionId;
            this.Id = configurationId;
        }

        internal static DocumentGenerationConfiguration Create(
            InternalClient internalClient,
            Guid subscriptionId,
            string name,
            string[] levelNames,
            bool hasLicense,
            string metadataFilenameExtension)
        {
            return new DocumentGenerationConfiguration(internalClient, subscriptionId, name, levelNames, hasLicense, metadataFilenameExtension);
        }

        private DocumentGenerationConfiguration(
            InternalClient internalClient,
            Guid subscriptionId,
            string name,
            string[] levelNames,
            bool hasLicense,
            string metadataFilenameExtension)
        {
            this.InternalClient = internalClient;
            this.SubscriptionId = subscriptionId;
            this.Name = name;
            this.LevelNames = levelNames;
            this.HasLicense = hasLicense;
            this.MetadataFilenameExtension = metadataFilenameExtension;
        }

        public async Task Save()
        {
            if (this.TemplateStorageDirectory == null)
            {
                throw new DocumentGenerationConfigurationException("Cannot Save without a TemplateStorageDirectory");
            }

            DocumentGenerationConfigurationSummary serverDocumentGenerationConfigurationSummary = null;
            if (this.Id != Guid.Empty)
            {
                serverDocumentGenerationConfigurationSummary =
                    await this.InternalClient
                        .GetConfigurationSummaryByIdWithHttpMessagesAsync(this.SubscriptionId, this.Id)
                        .ValidateBody()
                        .ConfigureAwait(false);
            }

            if (serverDocumentGenerationConfigurationSummary == null)
            {
                throw new DocumentGenerationConfigurationException("Cannot Save a Configuration unless it has already been created on the server.");
            }

            var serverDocumentGenerationConfigurationSkeleton = new DocumentGenerationConfigurationSkeleton(this.InternalClient, serverDocumentGenerationConfigurationSummary, true);
            this.Id = serverDocumentGenerationConfigurationSkeleton.Id;
            await this.InternalClient.UpdateDocumentGenerationConfigurationWithHttpMessagesAsync(
                    this.SubscriptionId,
                    this.Id,
                    new UpdateConfigurationRequest(
                        this.Name,
                        this.HasLicense,
                        this.LevelNames,
                        this.MetadataFilenameExtension))
                .ValidateBody()
                .ConfigureAwait(false);
            await this.TemplateStorageDirectory.Save(serverDocumentGenerationConfigurationSkeleton.TemplateStorageDirectory).ConfigureAwait(false);
        }

        public async Task<DocumentGenerationConfiguration> Load()
        {
            var serverDocumentGenerationConfigurationSummary =
                await this.InternalClient
                    .GetConfigurationSummaryByIdWithHttpMessagesAsync(this.SubscriptionId, this.Id)
                    .ValidateBody()
                    .ConfigureAwait(false);

            var documentGenerationConfigurationSkeleton = new DocumentGenerationConfigurationSkeleton(this.InternalClient, serverDocumentGenerationConfigurationSummary, false);
            this.UpdateProperties(documentGenerationConfigurationSkeleton);
            var documentGenerationConfigurationEntrySkeleton = documentGenerationConfigurationSkeleton.TemplateStorageDirectory;
            if (documentGenerationConfigurationEntrySkeleton != null)
            {
                var entryId = documentGenerationConfigurationEntrySkeleton.Id;
                if (this.TemplateStorageDirectory != null && this.TemplateStorageDirectory.Id == entryId)
                {
                    await this.TemplateStorageDirectory.Load(documentGenerationConfigurationEntrySkeleton).ConfigureAwait(false);
                }
                else
                {
                    var templateStorageDirectory = new DocumentGenerationTemplateStorageDirectory(this, null, documentGenerationConfigurationEntrySkeleton);
                    await templateStorageDirectory.Load(documentGenerationConfigurationEntrySkeleton).ConfigureAwait(false);
                    this.TemplateStorageDirectory = templateStorageDirectory;
                }
            }
            else
            {
                this.TemplateStorageDirectory = null;
            }

            return this;
        }

        public async Task Delete()
        {
            await this.InternalClient
                .DeleteDocumentGenerationConfigurationWithHttpMessagesAsync(this.SubscriptionId, this.Id)
                .ValidateResponse()
                .ConfigureAwait(false);
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

        public Task<DocumentGenerationProgress> GetDocumentGenerationProgress(Guid documentGenerationRequestId)
        {
            var documentGenerationRequest =
                this.InternalClient
                    .GetDocumentGenerationWithHttpMessagesAsync(this.SubscriptionId, documentGenerationRequestId)
                    .ValidateBody();
            return documentGenerationRequest.ToDocumentGenerationProgress();
        }

        public Task<DocumentGenerationUri> GetDocumentGenerationUri(Guid documentGenerationRequestId)
        {
            var documentUri =
                this.InternalClient
                    .GetDocumentWithHttpMessagesAsync(this.SubscriptionId, documentGenerationRequestId)
                    .ValidateBody();
            return documentUri.ToDocumentGenerationUri();
        }

        public Task<DocumentGenerationProgress> RequestDocumentConversionToPdfA(DocumentConversionToPdfARequestDetails documentConversionToPdfARequestDetails)
        {
            var documentGenerationRequest =
                this.InternalClient.RequestDocumentConversionWithHttpMessagesAsync(this.SubscriptionId, documentConversionToPdfARequestDetails.ToWebRequest(this.Id))
                    .ValidateBody();
            return documentGenerationRequest.ToDocumentGenerationProgress();
        }

        public Task<DocumentGenerationProgress> RequestDocumentConversion(DocumentConversionRequestDetails documentConversionRequestDetails)
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
            this.MetadataFilenameExtension = documentGenerationConfigurationSummary.MetadataFilenameExtension;
            this.HasLicense = documentGenerationConfigurationSummary.HasLicense;
        }
    }
}
