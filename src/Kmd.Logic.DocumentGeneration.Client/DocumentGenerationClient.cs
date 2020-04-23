using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.DocumentGeneration.Client.Configuration;
using Kmd.Logic.DocumentGeneration.Client.Models;
using Kmd.Logic.DocumentGeneration.Client.ModelTranslator;
using Kmd.Logic.DocumentGeneration.Client.ServiceMessages;
using Kmd.Logic.DocumentGeneration.Client.Types;
using Kmd.Logic.Identity.Authorization;
using Microsoft.Rest;
using Newtonsoft.Json.Linq;

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
        /// <param name="subscriptionId">Identifier of Logic subscription.</param>
        /// <param name="configurationId">Identifier of document generation configuration to be used.</param>
        /// <param name="hierarchyPath">The hierarchy of possible template sources not including the master location.
        /// For example, if you have a customer "A0001" with a department "B0001" then the hierarchy path would be @"\A0001\B0001".
        /// If the department has no template source configured then the customers templates will be used.</param>
        /// <param name="templateId">Identifier of template to be used.</param>
        /// <param name="twoLetterIsoLanguageName">Language code in ISO 639-2 format (eg. en).</param>
        /// <param name="documentFormat">Format of the generated document. Possible values include: 'Txt', 'Rtf', 'Doc', 'Docx', 'Pdf'.</param>
        /// <param name="mergeData">Data to be merged into document.</param>
        /// <param name="callbackUrl">URL that is going to be called when document generation completes.</param>
        /// <param name="debug">Flag indicating if document generation should be run in diagnostic mode.</param>
        /// <returns>DocumentGenerationProgress object.</returns>
        public DocumentGenerationProgress RequestDocumentGeneration(
            Guid? subscriptionId,
            Guid configurationId,
            string hierarchyPath,
            string templateId,
            string twoLetterIsoLanguageName,
            DocumentFormat documentFormat,
            JObject mergeData,
            Uri callbackUrl,
            bool? debug)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);
            return this.Client.RequestDocumentGeneration(
                    resolvedSubscriptionId,
                    new GenerateDocumentRequest(
                        configurationId,
                        hierarchyPath,
                        templateId,
                        twoLetterIsoLanguageName,
                        documentFormat.ToString(),
                        mergeData))
                ?.ToDocumentGenerationProgress();
        }

        /// <summary>
        /// Gets document generation progress information.
        /// </summary>
        /// <param name="subscriptionId">Identifier of Logic subscription.</param>
        /// <param name="documentGenerationRequestId">Identifier of request about which to return progress information.</param>
        /// <returns>Document generation request.</returns>
        public DocumentGenerationProgress GetDocumentGenerationProgress(Guid? subscriptionId, Guid documentGenerationRequestId)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);
            var documentGenerationRequest =
                this.Client.GetDocumentGeneration(resolvedSubscriptionId, documentGenerationRequestId);
            return documentGenerationRequest.ToDocumentGenerationProgress();
        }

        /// <summary>
        /// Gets a Uri to a generated document associated with the provide request id.
        /// </summary>
        /// <param name="subscriptionId">Identifier of Logic subscription.</param>
        /// <param name="documentGenerationRequestId">Identifier of request which document should be retuned.</param>
        /// <returns>DocumentUri of the generated document.</returns>
        public DocumentGenerationUri GetDocumentGenerationUri(Guid? subscriptionId, Guid documentGenerationRequestId)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);
            var documentUri =
                this.Client
                    .GetDocument(resolvedSubscriptionId, documentGenerationRequestId);
            return documentUri?.ToDocumentGenerationUri();
        }

        /// <summary>
        /// Writes document generated for provided request to the output stream provided.
        /// </summary>
        /// <param name="subscriptionId">Identifier of Logic subscription.</param>
        /// <param name="requestId">Identifier of request which document should be retuned.</param>
        /// <param name="outputStream">Output stream to which to write the generated document.</param>
        /// <returns>void.</returns>
        public async Task WriteDocumentToStreamAsync(Guid? subscriptionId, Guid requestId, Stream outputStream)
        {
            var documentUri = this.GetDocumentGenerationUri(subscriptionId, requestId);
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
        /// <param name="subscriptionId">Identifier of Logic subscription.</param>
        /// <param name="configurationId">Identifier of configuration to use.</param>
        /// <param name="hierarchyPath">The hierarchy of possible template sources not including the master location.
        /// For example, if you have a customer "A0001" with a department "B0001" then the hierarchy path would be @"\A0001\B0001".
        /// If the department has no template source configured then the customers templates will be used.</param>
        /// <param name="subject">Subject of created document.</param>
        /// <returns>List of templates that can be requested.</returns>
        public IEnumerable<DocumentGenerationTemplate> GetTemplates(Guid? subscriptionId, Guid configurationId, string hierarchyPath, string subject)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);
            return this.Client.GetTemplates(resolvedSubscriptionId, configurationId, hierarchyPath, subject)
                ?.Select(t => t.ToDocumentGenerationTemplate())
                .ToArray();
        }

        /// <summary>
        /// Get all document generation configurations managed by the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription that owns the configurations.</param>
        /// <returns>The list of existing configurations.</returns>
        public IEnumerable<DocumentGenerationConfigurationListItem> GetConfigurationsForSubscription(Guid? subscriptionId)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);
            return this.Client
                .GetAllConfigurations(resolvedSubscriptionId)
                .Select(c => c.ToDocumentGenerationConfigurationListItem());
        }

        /// <summary>
        /// Get a document generation configuration managed by the subscription by the id of the configuration.
        /// </summary>
        /// <param name="subscriptionId">The subscription that owns the configurations.</param>
        /// <param name="configurationId">Identifier of the configuration to return.</param>
        /// <returns>The requested document generation configuration object.</returns>
        public DocumentGenerationConfiguration GetDocumentGenerationConfiguration(Guid? subscriptionId, Guid configurationId)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);
            return new DocumentGenerationConfiguration(this.Client, resolvedSubscriptionId, configurationId);
        }

        /// <summary>
        /// Requests document conversion to Pdf/A.
        /// </summary>
        /// <param name="subscriptionId">Identifier of Logic subscription.</param>
        /// <param name="configurationId">Identifier of the Document Generation Configuration.</param>
        /// <param name="documentConversionToPdfARequest">Document conversion parameters.</param>
        /// <returns>DocumentGenerationProgress.</returns>
        public async Task<DocumentGenerationProgress> RequestDocumentConversionToPdfA(
            Guid? subscriptionId,
            Guid configurationId,
            DocumentConversionToPdfARequest documentConversionToPdfARequest)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);
            var documentGenerationRequest =
                await this.Client.RequestDocumentConversionAsync(resolvedSubscriptionId, documentConversionToPdfARequest.ToWebRequest(configurationId));
            return documentGenerationRequest.ToDocumentGenerationProgress();
        }

        /// <summary>
        /// Requests document generation.
        /// </summary>
        /// <param name="subscriptionId">Identifier of Logic subscription.</param>
        /// <param name="request">Document generation parameters.</param>
        /// <returns>DocumentGenerationRequest object.</returns>
        [Obsolete("Use RequestDocumentGeneration instead.")]
        public async Task<DocumentGenerationRequest> RequestDocumentGenerationAsync(Guid? subscriptionId, GenerateDocumentRequest request = default(GenerateDocumentRequest))
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);
            try
            {
                return await this.Client.RequestDocumentGenerationAsync(
                    subscriptionId: resolvedSubscriptionId, request).ConfigureAwait(false);
            }
            catch (ValidationException ex)
            {
                throw new DocumentGenerationValidationException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Gets document generation request.
        /// </summary>
        /// <param name="subscriptionId">Identifier of Logic subscription.</param>
        /// <param name="requestId">Identifier of request to return.</param>
        /// <returns>Document generation request.</returns>
        [Obsolete("Use GetDocumentGenerationProgress instead.")]
        public async Task<DocumentGenerationRequest> GetDocumentGenerationAsync(Guid? subscriptionId = null, Guid? requestId = null)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);
            return await this.Client
                .GetDocumentGenerationAsync(resolvedSubscriptionId, requestId: requestId ?? Guid.NewGuid())
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets document generated for provided request.
        /// </summary>
        /// <param name="subscriptionId">Identifier of Logic subscription.</param>
        /// <param name="requestId">Identifier of request which document should be retuned.</param>
        /// <returns>DocumentUri of the generated document.</returns>
        [Obsolete("Use GetDocumentGenerationUri instead")]
        public async Task<DocumentUri> GetDocumentAsync(Guid? subscriptionId, Guid requestId)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);

            return await this.Client
                .GetDocumentAsync(resolvedSubscriptionId, requestId: requestId)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// List all templates.
        /// </summary>
        /// <param name="subscriptionId">Identifier of Logic subscription.</param>
        /// <param name="configurationId">Identifier of configuration to use.</param>
        /// <param name="hierarchyPath">
        /// The hierarchy of possible template sources not including the master location.
        /// For example, if you have a customer "A0001" with a department "B0001" then the hierarchy path would be "A0001\B0001".
        /// If the department has no template source configured then the customers templates will be used.
        /// </param>
        /// <param name="subject">Subject of created document.</param>
        /// <returns>List of templates that can be requested.</returns>
        [Obsolete("Use GetTemplates instead")]
        public async Task<IEnumerable<Template>> GetTemplatesAsync(Guid? subscriptionId, Guid configurationId, string hierarchyPath, string subject)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);
            return await this.Client
                .GetTemplatesAsync(resolvedSubscriptionId, configurationId, hierarchyPath, subject)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get all document generation configurations managed by the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription that owns the configurations.</param>
        /// <returns>The list of existing configurations.</returns>
        [Obsolete("Use GetConfigurationsForSubscription instead.")]
        public async Task<IEnumerable<ConfigurationListResponse>> GetAllConfigurationsForSubscription(Guid? subscriptionId)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);
            return await this.Client
                .GetAllConfigurationsAsync(resolvedSubscriptionId)
                .ConfigureAwait(false);
        }
    }
}
