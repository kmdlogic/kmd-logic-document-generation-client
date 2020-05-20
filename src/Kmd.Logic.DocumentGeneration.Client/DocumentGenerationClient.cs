using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.DocumentGeneration.Client.Configuration;
using Kmd.Logic.DocumentGeneration.Client.ModelTranslator;
using Kmd.Logic.DocumentGeneration.Client.ServiceMessages;
using Kmd.Logic.DocumentGeneration.Client.Types;
using Kmd.Logic.Identity.Authorization;
using Microsoft.Rest;

namespace Kmd.Logic.DocumentGeneration.Client
{
    /// <summary>
    /// Manage distributed documentGeneration shared between multiple member systems.
    /// </summary>
    /// <remarks>
    /// To access the documentGeneration service you:
    /// - Create a Logic subscription
    /// - Have a client credential issued for the Logic platform
    /// - Create a DocumentGeneration Group, nominating one or more members with what permissions they have.
    /// </remarks>
    [SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "HttpClient is not owned by this class.")]
    public sealed class DocumentGenerationClient : LogicHttpClientProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentGenerationClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use. The caller is expected to manage this resource and it will not be disposed.</param>
        /// <param name="tokenProviderFactory">The Logic access token provider factory.</param>
        /// <param name="options">The required configuration options.</param>
        public DocumentGenerationClient(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, DocumentGenerationOptions options)
            : base(httpClient, tokenProviderFactory, options)
        {
        }

        /// <summary>
        /// Requests document generation.
        /// </summary>
        /// <param name="configurationId">Identifier of document generation configuration to be used.</param>
        /// <param name="documentGenerationRequestDetails">Document generation parameters.</param>
        /// <returns>DocumentGenerationProgress object.</returns>
        public Task<DocumentGenerationProgress> RequestDocumentGeneration(
            Guid configurationId,
            DocumentGenerationRequestDetails documentGenerationRequestDetails)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId();
            try
            {
                var documentGenerationRequest =
                    this.Client.RequestDocumentGenerationWithHttpMessagesAsync(
                            resolvedSubscriptionId,
                            documentGenerationRequestDetails.ToWebRequest(configurationId));
                return documentGenerationRequest.ValidateBody().ToDocumentGenerationProgress();
            }
            catch (HttpOperationException httpOperationException)
            {
                throw httpOperationException.DocumentGenerationThrow();
            }
        }

        /// <summary>
        /// Gets document generation progress information.
        /// </summary>
        /// <param name="documentGenerationRequestId">Identifier of request about which to return progress information.</param>
        /// <returns>Document generation request.</returns>
        public Task<DocumentGenerationProgress> GetDocumentGenerationProgress(Guid documentGenerationRequestId)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId();
            try
            {
                var documentGenerationRequest =
                    this.Client.GetDocumentGenerationWithHttpMessagesAsync(resolvedSubscriptionId, documentGenerationRequestId);
                return documentGenerationRequest.ValidateBody().ToDocumentGenerationProgress();
            }
            catch (HttpOperationException httpOperationException)
            {
                throw httpOperationException.DocumentGenerationThrow();
            }
        }

        /// <summary>
        /// Gets a Uri to a generated document associated with the provide request id.
        /// </summary>
        /// <param name="documentGenerationRequestId">Identifier of request which document should be retuned.</param>
        /// <returns>DocumentUri of the generated document.</returns>
        public Task<DocumentGenerationUri> GetDocumentGenerationUri(Guid documentGenerationRequestId)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId();
            try
            {
                var documentUri =
                    this.Client
                        .GetDocumentWithHttpMessagesAsync(resolvedSubscriptionId, documentGenerationRequestId);
                return documentUri.ValidateBody().ToDocumentGenerationUri();
            }
            catch (HttpOperationException httpOperationException)
            {
                throw httpOperationException.DocumentGenerationThrow();
            }
        }

        /// <summary>
        /// Writes document generated for provided request to the output stream provided.
        /// </summary>
        /// <param name="requestId">Identifier of request which document should be retuned.</param>
        /// <param name="outputStream">Output stream to which to write the generated document.</param>
        /// <returns>void.</returns>
        public async Task WriteDocumentToStreamAsync(Guid requestId, Stream outputStream)
        {
            var documentUri = await this.GetDocumentGenerationUri(requestId).ConfigureAwait(false);
            if (documentUri == null)
            {
                throw new DocumentGenerationException($"Unable to find generated document for request {requestId}.");
            }

            var response = await this.HttpClient.GetAsync(documentUri.Uri).ConfigureAwait(false);
            await response.Content.CopyToAsync(outputStream).ConfigureAwait(false);
        }

        /// <summary>
        /// List all templates.
        /// </summary>
        /// <param name="configurationId">Identifier of configuration to use.</param>
        /// <param name="hierarchyPath">The hierarchy of possible template sources not including the master location.
        /// For example, if you have a customer "A0001" with a department "B0001" then the hierarchy path would be @"\A0001\B0001".
        /// If the department has no template source configured then the customers templates will be used.</param>
        /// <param name="subject">Subject of created document.</param>
        /// <returns>List of templates that can be requested.</returns>
        public async Task<IEnumerable<DocumentGenerationTemplate>> GetTemplates(Guid configurationId, string hierarchyPath, string subject)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId();
            try
            {
                var templates =
                    await this.Client.GetTemplatesWithHttpMessagesAsync(
                            resolvedSubscriptionId,
                            configurationId,
                            hierarchyPath,
                            subject)
                        .ValidateBody()
                        .ConfigureAwait(false);
                return templates.Select(t => t.ToDocumentGenerationTemplate()).ToArray();
            }
            catch (HttpOperationException httpOperationException)
            {
                throw httpOperationException.DocumentGenerationThrow();
            }
        }

        /// <summary>
        /// Get all document generation configurations managed by the subscription.
        /// </summary>
        /// <returns>The list of existing configurations.</returns>
        public async Task<IEnumerable<DocumentGenerationConfigurationListItem>> GetConfigurationsForSubscription()
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId();
            try
            {
                var configurations = await this.Client
                    .GetAllConfigurationsWithHttpMessagesAsync(resolvedSubscriptionId)
                    .ValidateBody()
                    .ConfigureAwait(false);
                return configurations.Select(c => c.ToDocumentGenerationConfigurationListItem());
            }
            catch (HttpOperationException httpOperationException)
            {
                throw httpOperationException.DocumentGenerationThrow();
            }
        }

        /// <summary>
        /// Get a document generation configuration managed by the subscription by the id of the configuration.
        /// </summary>
        /// <param name="configurationId">Identifier of the configuration to return.</param>
        /// <returns>The requested document generation configuration object.</returns>
        public Task<DocumentGenerationConfiguration> GetDocumentGenerationConfiguration(Guid configurationId)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId();
            try
            {
                var documentGenerationConfiguration =
                    new DocumentGenerationConfiguration(this.Client, resolvedSubscriptionId, configurationId);
                return documentGenerationConfiguration.Load();
            }
            catch (HttpOperationException httpOperationException)
            {
                throw httpOperationException.DocumentGenerationThrow();
            }
        }

        /// <summary>
        /// Requests document conversion to Pdf/A.
        /// </summary>
        /// <param name="configurationId">Identifier of the Document Generation Configuration.</param>
        /// <param name="documentConversionToPdfARequestDetails">Document conversion parameters.</param>
        /// <returns>DocumentGenerationProgress.</returns>
        public Task<DocumentGenerationProgress> RequestDocumentConversionToPdfA(
            Guid configurationId,
            DocumentConversionToPdfARequestDetails documentConversionToPdfARequestDetails)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId();
            try
            {
                var documentGenerationRequest =
                    this.Client.RequestDocumentConversionWithHttpMessagesAsync(resolvedSubscriptionId, documentConversionToPdfARequestDetails.ToWebRequest(configurationId));
                return documentGenerationRequest.ValidateBody().ToDocumentGenerationProgress();
            }
            catch (HttpOperationException httpOperationException)
            {
                throw httpOperationException.DocumentGenerationThrow();
            }
        }

        /// <summary>
        /// Gets metadata for a nominated template.
        /// </summary>
        /// <param name="configurationId">Identifier of configuration to use.</param>
        /// <param name="templateId">Identifier of the associated template.</param>
        /// <param name="language">Language code in ISO 639-2 format (e.g. en).</param>
        /// <param name="hierarchyPath">
        /// The hierarchy of possible template sources not including the master location.
        /// For example, if you have a customer "A0001" with a department "B0001" then the hierarchy path would be "A0001\B0001".
        /// If the department has no template source configured then the customers templates will be used.
        /// </param>
        /// <returns>The template metadata as an output stream.</returns>
        public async Task<Stream> GetMetadata(Guid configurationId, string templateId, string language, string hierarchyPath)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId();
            try
            {
                var response =
                    await this.Client.GetMetadataWithHttpMessagesAsync(
                            resolvedSubscriptionId,
                            configurationId,
                            templateId,
                            language,
                            hierarchyPath)
                        .ConfigureAwait(false);
                return await response.ValidateContentStream().ConfigureAwait(false);
            }
            catch (HttpOperationException httpOperationException)
            {
                throw httpOperationException.DocumentGenerationThrow();
            }
        }
    }
}
