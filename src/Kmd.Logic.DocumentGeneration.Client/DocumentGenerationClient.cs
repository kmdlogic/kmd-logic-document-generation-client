using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Kmd.Logic.DocumentGeneration.Client.Models;
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
    public sealed class DocumentGenerationClient
    {
        private readonly HttpClient _httpClient;
        private readonly DocumentGenerationOptions _options;
        private readonly LogicTokenProviderFactory _tokenProviderFactory;

        private InternalClient _internalClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentGenerationClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use. The caller is expected to manage this resource and it will not be disposed.</param>
        /// <param name="tokenProviderFactory">The Logic access token provider factory.</param>
        /// <param name="options">The required configuration options.</param>
        public DocumentGenerationClient(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, DocumentGenerationOptions options)
        {
            this._httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this._options = options ?? throw new ArgumentNullException(nameof(options));
            this._tokenProviderFactory = tokenProviderFactory ?? throw new ArgumentNullException(nameof(tokenProviderFactory));

#pragma warning disable CS0618 // Type or member is obsolete
            if (string.IsNullOrEmpty(this._tokenProviderFactory.DefaultAuthorizationScope))
            {
                this._tokenProviderFactory.DefaultAuthorizationScope = "https://logicidentityprod.onmicrosoft.com/bb159109-0ccd-4b08-8d0d-80370cedda84/.default";
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Requests document generation.
        /// </summary>
        /// <param name="subscriptionId">Identifier of Logic subscription.</param>
        /// <param name="request">Document generation parameters.</param>
        /// <returns>DocumentGenerationRequest object.</returns>
        public async Task<DocumentGenerationRequest> RequestDocumentGenerationAsync(Guid? subscriptionId, GenerateDocumentRequest request = default(GenerateDocumentRequest))
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);
            var client = this.CreateClient();
            try
            {
                return await client.RequestDocumentGenerationAsync(
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
        public async Task<DocumentGenerationRequest> GetDocumentGenerationAsync(Guid? subscriptionId = null, Guid? requestId = null)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);
            var client = this.CreateClient();
            return await client
                .GetDocumentGenerationAsync(resolvedSubscriptionId, requestId: requestId ?? Guid.NewGuid())
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets document generated for provided request.
        /// </summary>
        /// <param name="subscriptionId">Identifier of Logic subscription.</param>
        /// <param name="requestId">Identifier of request which document should be retuned.</param>
        /// <returns>DocumentUri of the generated document.</returns>
        public async Task<DocumentUri> GetDocumentAsync(Guid? subscriptionId, Guid requestId)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);

            var client = this.CreateClient();

            return await client
                .GetDocumentAsync(resolvedSubscriptionId, requestId: requestId)
                .ConfigureAwait(false);
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
            var documentUri = await this.GetDocumentAsync(subscriptionId, requestId).ConfigureAwait(false);
            var response = await this._httpClient.GetAsync(documentUri.Uri).ConfigureAwait(false);
            await response.Content.CopyToAsync(outputStream).ConfigureAwait(false);
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
        public async Task<IEnumerable<Template>> GetTemplatesAsync(Guid? subscriptionId, Guid configurationId, string hierarchyPath, string subject)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);
            var client = this.CreateClient();
            return await client
                .GetTemplatesAsync(resolvedSubscriptionId, configurationId, hierarchyPath, subject)
                .ConfigureAwait(false);
        }

        private InternalClient CreateClient()
        {
            if (this._internalClient != null)
            {
                return this._internalClient;
            }

            var tokenProvider = this._tokenProviderFactory.GetProvider(this._httpClient);

            this._internalClient = new InternalClient(new TokenCredentials(tokenProvider))
            {
                BaseUri = this._options.DocumentGenerationServiceUri,
            };

            return this._internalClient;
        }

        private Guid ResolveSubscriptionId(Guid? subscriptionId)
        {
            var resolvedSubscriptionId = subscriptionId ?? this._options.SubscriptionId;
            if (resolvedSubscriptionId == null)
            {
                throw new DocumentGenerationValidationException("No subscription id provided", new Dictionary<string, IList<string>>());
            }

            return resolvedSubscriptionId.Value;
        }
    }
}
