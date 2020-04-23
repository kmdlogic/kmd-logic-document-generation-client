// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Kmd.Logic.DocumentGeneration.Client
{
    using Microsoft.Rest;
    using Models;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// </summary>
    internal partial interface IInternalClient : System.IDisposable
    {
        /// <summary>
        /// The base URI of the service.
        /// </summary>
        System.Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets json serialization settings.
        /// </summary>
        JsonSerializerSettings SerializationSettings { get; }

        /// <summary>
        /// Gets or sets json deserialization settings.
        /// </summary>
        JsonSerializerSettings DeserializationSettings { get; }

        /// <summary>
        /// Subscription credentials which uniquely identify client
        /// subscription.
        /// </summary>
        ServiceClientCredentials Credentials { get; }


        /// <summary>
        /// Gets a SharePoint Online template storage configuration entry for a
        /// given document generation configuration managed by the
        /// subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to fetch.
        /// </param>
        /// <param name='hierarchyPath'>
        /// The configuration entry ancestry path to the entry to be returned.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<AzureBlobStorageConfigurationDocumentGenerationConfigurationEntryDetails>> GetAzureBlobEntryAtPathWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, string hierarchyPath = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates a SharePoint Online template storage configuration entry
        /// for a given document generation configuration managed by the
        /// subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to fetch.
        /// </param>
        /// <param name='hierarchyPath'>
        /// The configuration entry ancestry path to the entry.
        /// </param>
        /// <param name='request'>
        /// SharePoint Online template storage settings.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<AzureBlobStorageConfigurationDocumentGenerationConfigurationEntryDetails>> UpdateAzureBlobEntryAtPathWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, string hierarchyPath = default(string), AzureBlobTemplateModelUpdateEntryAtPathRequest request = default(AzureBlobTemplateModelUpdateEntryAtPathRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a SharePoint Online template storage configuration entry
        /// for a given document generation configuration managed by the
        /// subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to fetch.
        /// </param>
        /// <param name='parentHierarchyPath'>
        /// The configuration entry ancestry path to the parent entry.
        /// </param>
        /// <param name='request'>
        /// SharePoint Online template storage settings.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<AzureBlobStorageConfigurationDocumentGenerationConfigurationEntryDetails>> CreateAzureBlobEntryUnderPathWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, string parentHierarchyPath = default(string), AzureBlobTemplateModelCreateEntryUnderPathRequest request = default(AzureBlobTemplateModelCreateEntryUnderPathRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a storage configuration entry for a given document
        /// generation configuration managed by the subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configuration.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to having the entry to be deleted.
        /// </param>
        /// <param name='hierarchyPath'>
        /// The configuration entry ancestry path to the entry to be deleted.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<DocumentGenerationConfigurationSummary>> DeleteAzureBlobEntryAtPathWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, string hierarchyPath = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a SharePoint Online template storage configuration entry for a
        /// given document generation configuration managed by the
        /// subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to fetch.
        /// </param>
        /// <param name='entryId'>
        /// The configuration entry identifier of the entry to be returned.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<AzureBlobStorageConfigurationDocumentGenerationConfigurationEntryDetails>> GetAzureBlobEntryAtIdWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, System.Guid entryId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates a SharePoint Online template storage configuration entry
        /// for a given document generation configuration managed by the
        /// subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to fetch.
        /// </param>
        /// <param name='entryId'>
        /// The configuration entry identifier of the entry to be updated.
        /// </param>
        /// <param name='request'>
        /// SharePoint Online template storage settings.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<AzureBlobStorageConfigurationDocumentGenerationConfigurationEntryDetails>> UpdateAzureBlobEntryAtIdWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, System.Guid entryId, AzureBlobTemplateModelUpdateEntryAtIdRequest request = default(AzureBlobTemplateModelUpdateEntryAtIdRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a storage configuration entry for a given document
        /// generation configuration managed by the subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configuration.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to having the entry to be deleted.
        /// </param>
        /// <param name='entryId'>
        /// The configuration entry identifier of the entry to be deleted.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<DocumentGenerationConfigurationSummary>> DeleteAzureBlobEntryAtIdWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, System.Guid entryId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a SharePoint Online template storage configuration entry
        /// for a given document generation configuration managed by the
        /// subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to fetch.
        /// </param>
        /// <param name='parentEntryId'>
        /// The configuration entry identifier of the parent entry.
        /// </param>
        /// <param name='request'>
        /// SharePoint Online template storage settings.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<AzureBlobStorageConfigurationDocumentGenerationConfigurationEntryDetails>> CreateAzureBlobEntryUnderIdWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, System.Guid parentEntryId, AzureBlobTemplateModelCreateEntryUnderIdRequest request = default(AzureBlobTemplateModelCreateEntryUnderIdRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a SharePoint Online template storage configuration entry for a
        /// given document generation configuration managed by the
        /// subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to fetch.
        /// </param>
        /// <param name='hierarchyPath'>
        /// The configuration entry ancestry path to the entry to be returned.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<SharePointOnlineTemplateStorageConfigurationDocumentGenerationConfigurationEntryDetails>> GetSharePointOnlineEntryAtPathWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, string hierarchyPath = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates a SharePoint Online template storage configuration entry
        /// for a given document generation configuration managed by the
        /// subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to fetch.
        /// </param>
        /// <param name='hierarchyPath'>
        /// The configuration entry ancestry path to the entry.
        /// </param>
        /// <param name='request'>
        /// SharePoint Online template storage settings.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<SharePointOnlineTemplateStorageConfigurationDocumentGenerationConfigurationEntryDetails>> UpdateSharePointOnlineEntryAtPathWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, string hierarchyPath = default(string), SharePointOnlineTemplateModelUpdateEntryAtPathRequest request = default(SharePointOnlineTemplateModelUpdateEntryAtPathRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a SharePoint Online template storage configuration entry
        /// for a given document generation configuration managed by the
        /// subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to fetch.
        /// </param>
        /// <param name='parentHierarchyPath'>
        /// The configuration entry ancestry path to the parent entry.
        /// </param>
        /// <param name='request'>
        /// SharePoint Online template storage settings.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<SharePointOnlineTemplateStorageConfigurationDocumentGenerationConfigurationEntryDetails>> CreateSharePointOnlineEntryUnderPathWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, string parentHierarchyPath = default(string), SharePointOnlineTemplateModelCreateEntryUnderPathRequest request = default(SharePointOnlineTemplateModelCreateEntryUnderPathRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a storage configuration entry for a given document
        /// generation configuration managed by the subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configuration.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to having the entry to be deleted.
        /// </param>
        /// <param name='hierarchyPath'>
        /// The configuration entry ancestry path to the entry to be deleted.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<DocumentGenerationConfigurationSummary>> DeleteSharePointOnlineEntryAtPathWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, string hierarchyPath = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a SharePoint Online template storage configuration entry for a
        /// given document generation configuration managed by the
        /// subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to fetch.
        /// </param>
        /// <param name='entryId'>
        /// The configuration entry identifier of the entry to be returned.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<SharePointOnlineTemplateStorageConfigurationDocumentGenerationConfigurationEntryDetails>> GetSharePointOnlineEntryAtIdWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, System.Guid entryId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Updates a SharePoint Online template storage configuration entry
        /// for a given document generation configuration managed by the
        /// subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to fetch.
        /// </param>
        /// <param name='entryId'>
        /// The configuration entry identifier of the entry to be updated.
        /// </param>
        /// <param name='request'>
        /// SharePoint Online template storage settings.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<SharePointOnlineTemplateStorageConfigurationDocumentGenerationConfigurationEntryDetails>> UpdateSharePointOnlineEntryAtIdWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, System.Guid entryId, SharePointOnlineTemplateModelUpdateEntryAtIdRequest request = default(SharePointOnlineTemplateModelUpdateEntryAtIdRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a storage configuration entry for a given document
        /// generation configuration managed by the subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configuration.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to having the entry to be deleted.
        /// </param>
        /// <param name='entryId'>
        /// The configuration entry identifier of the entry to be deleted.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<DocumentGenerationConfigurationSummary>> DeleteSharePointOnlineEntryAtIdWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, System.Guid entryId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a SharePoint Online template storage configuration entry
        /// for a given document generation configuration managed by the
        /// subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to fetch.
        /// </param>
        /// <param name='parentEntryId'>
        /// The configuration entry identifier of the parent entry.
        /// </param>
        /// <param name='request'>
        /// SharePoint Online template storage settings.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<SharePointOnlineTemplateStorageConfigurationDocumentGenerationConfigurationEntryDetails>> CreateSharePointOnlineEntryUnderIdWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, System.Guid parentEntryId, SharePointOnlineTemplateModelCreateEntryUnderIdRequest request = default(SharePointOnlineTemplateModelCreateEntryUnderIdRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get all document generation configurations managed by the
        /// subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<IList<ConfigurationListResponse>>> GetAllConfigurationsWithHttpMessagesAsync(System.Guid subscriptionId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Create a new document generation configuration.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='request'>
        /// The details of the configuration being created.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<DocumentGenerationConfigurationSummary>> CreateDocumentGenerationConfigurationWithHttpMessagesAsync(System.Guid subscriptionId, CreateConfigurationRequest request = default(CreateConfigurationRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Get the summary of a document generation configuration managed by
        /// the subscription by the id of the configuration.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to fetch.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<DocumentGenerationConfigurationSummary>> GetConfigurationSummaryByIdWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Update an existing document generation configuration.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to update.
        /// </param>
        /// <param name='request'>
        /// The details of the configuration being updated.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<DocumentGenerationConfigurationSummary>> UpdateDocumentGenerationConfigurationWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, UpdateConfigurationRequest request = default(UpdateConfigurationRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes a document generation configuration managed by the
        /// subscription.
        /// </summary>
        /// <param name='subscriptionId'>
        /// The subscription that owns the configurations.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of the configuration to fetch.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse> DeleteDocumentGenerationConfigurationWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Requests document generation.
        /// </summary>
        /// <param name='subscriptionId'>
        /// Identifier of Logic subscription.
        /// </param>
        /// <param name='request'>
        /// Document generation parameters.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<DocumentGenerationRequest>> RequestDocumentGenerationWithHttpMessagesAsync(System.Guid subscriptionId, GenerateDocumentRequest request = default(GenerateDocumentRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Requests document generation.
        /// </summary>
        /// <param name='subscriptionId'>
        /// Identifier of Logic subscription.
        /// </param>
        /// <param name='request'>
        /// Document generation parameters.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<DocumentGenerationRequest>> RequestDocumentConversionWithHttpMessagesAsync(System.Guid subscriptionId, GenerateDocumentConversionRequest request = default(GenerateDocumentConversionRequest), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets document generation request.
        /// </summary>
        /// <param name='subscriptionId'>
        /// Identifier of Logic subscription.
        /// </param>
        /// <param name='requestId'>
        /// Identifier of request to return.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<DocumentGenerationRequest>> GetDocumentGenerationWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid requestId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets URI to document generated for provided request.
        /// </summary>
        /// <param name='subscriptionId'>
        /// Identifier of Logic subscription.
        /// </param>
        /// <param name='requestId'>
        /// Identifier of request which document should be retuned.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<DocumentUri>> GetDocumentWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid requestId, Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// List all templates.
        /// </summary>
        /// <param name='subscriptionId'>
        /// Identifier of Logic subscription.
        /// </param>
        /// <param name='configurationId'>
        /// Identifier of configuration to use.
        /// </param>
        /// <param name='hierarchyPath'>
        /// The hierarchy of possible template sources not including the master
        /// location.
        /// For example, if you have a customer "A0001" with a department
        /// "B0001" then the hierarchy path would be "A0001\B0001".
        /// If the department has no template source configured then the
        /// customers templates will be used.
        /// </param>
        /// <param name='subject'>
        /// Subject of created document.
        /// </param>
        /// <param name='customHeaders'>
        /// The headers that will be added to request.
        /// </param>
        /// <param name='cancellationToken'>
        /// The cancellation token.
        /// </param>
        Task<HttpOperationResponse<IList<Template>>> GetTemplatesWithHttpMessagesAsync(System.Guid subscriptionId, System.Guid configurationId, string hierarchyPath = default(string), string subject = default(string), Dictionary<string, List<string>> customHeaders = null, CancellationToken cancellationToken = default(CancellationToken));

    }
}
