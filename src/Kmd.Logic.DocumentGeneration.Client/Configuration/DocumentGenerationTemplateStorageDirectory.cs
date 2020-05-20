using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Kmd.Logic.DocumentGeneration.Client.Configuration.TemplateStorageConfigurations;
using Kmd.Logic.DocumentGeneration.Client.ModelTranslator;
using Kmd.Logic.DocumentGeneration.Client.ServiceMessages;
using Kmd.Logic.DocumentGeneration.Client.Types;
using Newtonsoft.Json.Linq;

namespace Kmd.Logic.DocumentGeneration.Client.Configuration
{
    public class DocumentGenerationTemplateStorageDirectory
    {
        public Guid Id { get; private set; }

        public string Key { get; set; }

        public string Name { get; set; }

        private ITemplateStorageConfiguration _templateStorageConfiguration;

        public ITemplateStorageConfiguration TemplateStorageConfiguration
        {
            get => this._templateStorageConfiguration;
            set => this._templateStorageConfiguration = value ?? throw new DocumentGenerationConfigurationException("TemplateStorageConfiguration cannot be null");
        }

        private List<DocumentGenerationTemplateStorageDirectory> _children;

        public IReadOnlyList<DocumentGenerationTemplateStorageDirectory> Children
        {
            get => this._children.AsReadOnly();
            private set => this._children = new List<DocumentGenerationTemplateStorageDirectory>(value ?? Enumerable.Empty<DocumentGenerationTemplateStorageDirectory>());
        }

        private DocumentGenerationConfiguration _documentGenerationConfiguration;
        private DocumentGenerationTemplateStorageDirectory _parentEntry;

        private DocumentGenerationTemplateStorageDirectory(
            DocumentGenerationConfiguration documentGenerationConfiguration,
            string key,
            string name,
            ITemplateStorageConfiguration templateStorageConfiguration)
        {
            this._documentGenerationConfiguration = documentGenerationConfiguration;
            this.Key = key;
            this.Name = name;
            this.TemplateStorageConfiguration = templateStorageConfiguration;
            this.Children = null;
        }

        internal DocumentGenerationTemplateStorageDirectory(
            DocumentGenerationConfiguration documentGenerationConfiguration,
            DocumentGenerationTemplateStorageDirectory parentEntry,
            DocumentGenerationTemplateStorageDirectorySkeleton documentGenerationTemplateStorageDirectorySkeleton)
        {
            this._documentGenerationConfiguration = documentGenerationConfiguration;
            this.Id = documentGenerationTemplateStorageDirectorySkeleton.Id;
            this.Children = null;
            this._parentEntry = parentEntry;
        }

        internal static DocumentGenerationTemplateStorageDirectory Create(
            DocumentGenerationConfiguration documentGenerationConfiguration,
            string key,
            string name,
            ITemplateStorageConfiguration templateStorageConfiguration)
        {
            return new DocumentGenerationTemplateStorageDirectory(documentGenerationConfiguration, key, name, templateStorageConfiguration);
        }

        public void UpdateProperties(string key, string name, ITemplateStorageConfiguration templateStorageConfiguration)
        {
            this.Key = key;
            this.Name = name;
            this.TemplateStorageConfiguration = templateStorageConfiguration;
        }

        private void UpdateProperties(DocumentGenerationTemplateStorageDirectoryDetails details)
        {
            this.Key = details.Key;
            this.Name = details.Name;
            this.TemplateStorageConfiguration = details.TemplateStorageConfiguration;
        }

        public DocumentGenerationTemplateStorageDirectory AddChild(DocumentGenerationTemplateStorageDirectory child)
        {
            child._parentEntry = this;
            if (this._children.Any(c => c.Key == child.Key))
            {
                throw new DocumentGenerationConfigurationException($"A child entry with Key: {child.Key} already exists");
            }

            this._children.Add(child);
            return child;
        }

        public DocumentGenerationTemplateStorageDirectory AddChild(
            string key,
            string name,
            ITemplateStorageConfiguration templateStorageConfiguration)
        {
            var child =
                this._documentGenerationConfiguration.CreateDocumentGenerationTemplateStorageDirectory(key, name, templateStorageConfiguration);
            return this.AddChild(child);
        }

        public HierarchyPath HierarchyPath => this.ParentHierarchyPath.Add(this.Key);

        public HierarchyPath ParentHierarchyPath => this._parentEntry?.HierarchyPath ?? new HierarchyPath(Array.Empty<string>());

        internal async Task Save(DocumentGenerationTemplateStorageDirectorySkeleton serverDocumentGenerationTemplateStorageDirectorySkeleton)
        {
            if (serverDocumentGenerationTemplateStorageDirectorySkeleton == null)
            {
                var details = await this.CreateEntryOnServer().ConfigureAwait(false);
                this.Id = details.Id;
            }
            else
            {
                var details = await this.UpdateEntryOnServer(serverDocumentGenerationTemplateStorageDirectorySkeleton).ConfigureAwait(false);
                this.Id = details.Id;
            }

            var serverChildrenLookupById =
                serverDocumentGenerationTemplateStorageDirectorySkeleton?.Children
                    .ToDictionary(s => s.Id, s => s)
                ?? new Dictionary<Guid, DocumentGenerationTemplateStorageDirectorySkeleton>();
            var hereChildrenLookupByKey =
                this.Children.ToDictionary(s => s.Key, s => s);
            var hereChildrenIds = new HashSet<Guid>(this.Children.Select(c => c.Id));

            var deletedOnServer = new Dictionary<Guid, DocumentGenerationTemplateStorageDirectorySkeleton>();

            foreach (var serverChildPair in serverChildrenLookupById)
            {
                var serverId = serverChildPair.Key;
                var serverKey = serverChildPair.Value.Key;
                if (!hereChildrenIds.Contains(serverId))
                {
                    await serverChildPair.Value.DeleteOnServer().ConfigureAwait(false);
                    deletedOnServer[serverId] = serverChildPair.Value;
                    continue;
                }

                if (hereChildrenLookupByKey.TryGetValue(serverKey, out var hereChildClashingKey))
                {
                    if (hereChildClashingKey.Id != serverId)
                    {
                        await serverChildPair.Value.DeleteOnServer().ConfigureAwait(false);
                        deletedOnServer[serverId] = serverChildPair.Value;
                    }
                }
            }

            foreach (var id in deletedOnServer.Keys)
            {
                serverChildrenLookupById.Remove(id);
            }

            foreach (var hereChildPair in hereChildrenLookupByKey)
            {
                await hereChildPair.Value.Save(serverChildrenLookupById.TryGetValue(hereChildPair.Value.Id, out var entrySkeleton)
                    ? entrySkeleton
                    : null)
                    .ConfigureAwait(false);
            }
        }

        internal async Task Load(
            DocumentGenerationTemplateStorageDirectorySkeleton documentGenerationTemplateStorageDirectorySkeleton)
        {
            if (documentGenerationTemplateStorageDirectorySkeleton.IsDeprecated())
            {
                this.TemplateStorageConfiguration = new DeprecatedTemplateStorage();
                return;
            }

            var documentGenerationEntryDetails =
                await documentGenerationTemplateStorageDirectorySkeleton.GetEntryDetailsFromServer().ConfigureAwait(false);
            this.UpdateProperties(documentGenerationEntryDetails);

            var refreshedChildren = new List<DocumentGenerationTemplateStorageDirectory>();

            foreach (var childEntrySkeleton in documentGenerationTemplateStorageDirectorySkeleton.Children)
            {
                var child = this._children.FirstOrDefault(c => c.Key == childEntrySkeleton.Key);
                if (child != null)
                {
                    await child.Load(childEntrySkeleton).ConfigureAwait(false);
                }
                else
                {
                    child = new DocumentGenerationTemplateStorageDirectory(this._documentGenerationConfiguration, this, childEntrySkeleton);
                    await child.Load(childEntrySkeleton).ConfigureAwait(false);
                }

                refreshedChildren.Add(child);
            }

            this._children = refreshedChildren;
        }

        public DocumentGenerationTemplateStorageDirectory FindDirectoryByPath(HierarchyPath path)
        {
            if (path == null || path.Length == 0)
            {
                throw new DocumentGenerationConfigurationException("Empty path cannot reference a TemplateStorageDirectory");
            }

            if (path.Head != this.Key)
            {
                return null;
            }

            if (path.Length == 1)
            {
                return this;
            }

            return
                this.Children
                    .Select(c => c.FindDirectoryByPath(path.Tail))
                    .FirstOrDefault(c => c != null);
        }

        public async Task<IEnumerable<DocumentGenerationTemplate>> GetTemplates(string subject)
        {
            var templatesList = await
                this.InternalClient.GetTemplatesWithHttpMessagesAsync(this.SubscriptionId, this.ConfigurationId, this.HierarchyPath.ToString(), subject)
                    .ValidateBody()
                    .ConfigureAwait(false);
            return templatesList?.Select(t => t.ToDocumentGenerationTemplate())
                .ToArray();
        }

        public async Task<Stream> GetMetadata(string templateId, string twoLetterIsoLanguageName)
        {
            var response =
                await this.InternalClient.GetMetadataWithHttpMessagesAsync(
                    this.SubscriptionId,
                    this.ConfigurationId,
                    templateId,
                    twoLetterIsoLanguageName,
                    this.HierarchyPath.ToString());
            return await response.ValidateContentStream().ConfigureAwait(false);
        }

        public Task<DocumentGenerationProgress> RequestDocumentGeneration(string templateId, string twoLetterIsoLanguageName, DocumentFormat documentFormat, JObject mergeData, Uri callbackUrl, bool debug)
        {
            var internalDocumentGenerationRequest =
                this.InternalClient.RequestDocumentGenerationWithHttpMessagesAsync(
                        this.SubscriptionId,
                        new DocumentGenerationRequestDetails(
                                this.HierarchyPath.ToString(),
                                templateId,
                                twoLetterIsoLanguageName,
                                documentFormat,
                                mergeData,
                                callbackUrl,
                                debug)
                            .ToWebRequest(this.ConfigurationId))
                    .ValidateBody();
            return internalDocumentGenerationRequest
                .ToDocumentGenerationProgress();
        }

        public Guid SubscriptionId => this._documentGenerationConfiguration.SubscriptionId;

        public Guid ConfigurationId => this._documentGenerationConfiguration.Id;

        internal InternalClient InternalClient => this._documentGenerationConfiguration.InternalClient;
    }
}
