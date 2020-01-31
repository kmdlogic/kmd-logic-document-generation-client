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
        private readonly HttpClient httpClient;
        private readonly DocumentGenerationOptions options;
        private readonly LogicTokenProviderFactory tokenProviderFactory;

        private InternalClient internalClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentGenerationClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use. The caller is expected to manage this resource and it will not be disposed.</param>
        /// <param name="tokenProviderFactory">The Logic access token provider factory.</param>
        /// <param name="options">The required configuration options.</param>
        public DocumentGenerationClient(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, DocumentGenerationOptions options)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
            this.tokenProviderFactory = tokenProviderFactory ?? throw new ArgumentNullException(nameof(tokenProviderFactory));

#pragma warning disable CS0618 // Type or member is obsolete
            if (string.IsNullOrEmpty(this.tokenProviderFactory.DefaultAuthorizationScope))
            {
                this.tokenProviderFactory.DefaultAuthorizationScope = "https://logicidentityprod.onmicrosoft.com/bb159109-0ccd-4b08-8d0d-80370cedda84/.default";
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }

        public async Task<DocumentGenerationRequest> RequestDocumentGenerationAsync(System.Guid? subscriptionId, GenerateDocumentRequest request = default(GenerateDocumentRequest))
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
        public async Task<DocumentGenerationRequest> GetDocumentGenerationAsync(System.Guid? subscriptionId = null, System.Guid? requestId = null)
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
        /// <returns>Generated document.</returns>
        public async Task<Stream> GetDocumentAsync(System.Guid? subscriptionId, System.Guid requestId)
        {
            var resolvedSubscriptionId = this.ResolveSubscriptionId(subscriptionId);

            var client = this.CreateClient();

            try
            {
                var response = await client.GetDocumentWithHttpMessagesAsync(resolvedSubscriptionId, requestId: requestId).ConfigureAwait(false);
                switch (response.Response.StatusCode)
                {
                    case System.Net.HttpStatusCode.OK:
                        if (response.Response.Content == null)
                        {
                            return null;
                        }
                        else
                        {
                            return await response.Response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                        }

                    case System.Net.HttpStatusCode.NotFound:
                        return null;

                    default:
                        throw new DocumentGenerationConfigurationException("Invalid configuration provided to access documentGeneration service", response.Response.Content.ToString());
                }
            }
            catch (ValidationException ex)
            {
                throw new DocumentGenerationValidationException(ex.Message, ex);
            }
        }

        private InternalClient CreateClient()
        {
            if (this.internalClient != null)
            {
                return this.internalClient;
            }

            var tokenProvider = this.tokenProviderFactory.GetProvider(this.httpClient);

            this.internalClient = new InternalClient(new TokenCredentials(tokenProvider))
            {
                BaseUri = this.options.DocumentGenerationServiceUri,
            };

            return this.internalClient;
        }

        private Guid ResolveSubscriptionId(Guid? subscriptionId)
        {
            var resolvedSubscriptionId = subscriptionId ?? this.options.SubscriptionId;
            if (resolvedSubscriptionId == null)
            {
                throw new DocumentGenerationValidationException("No subscription id provided", new Dictionary<string, IList<string>>());
            }

            return resolvedSubscriptionId.Value;
        }
    }
}
