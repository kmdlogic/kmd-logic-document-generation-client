using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using Kmd.Logic.DocumentGeneration.Client.Types;
using Kmd.Logic.Identity.Authorization;
using Microsoft.Rest;

namespace Kmd.Logic.DocumentGeneration.Client
{
    [SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "HttpClient is not owned by this class.")]
    public class LogicHttpClientProvider
    {
        private readonly DocumentGenerationOptions _options;
        private readonly LogicTokenProviderFactory _tokenProviderFactory;

        private InternalClient _internalClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicHttpClientProvider"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client to use. The caller is expected to manage this resource and it will not be disposed.</param>
        /// <param name="tokenProviderFactory">The Logic access token provider factory.</param>
        /// <param name="options">The required configuration options.</param>
        protected LogicHttpClientProvider(HttpClient httpClient, LogicTokenProviderFactory tokenProviderFactory, DocumentGenerationOptions options)
        {
            this.HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this._options = options ?? throw new ArgumentNullException(nameof(options));
            this._tokenProviderFactory = tokenProviderFactory ?? throw new ArgumentNullException(nameof(tokenProviderFactory));

#pragma warning disable CS0618 // Type or member is obsolete
            if (string.IsNullOrEmpty(this._tokenProviderFactory.DefaultAuthorizationScope))
            {
                this._tokenProviderFactory.DefaultAuthorizationScope = "https://logicidentityprod.onmicrosoft.com/bb159109-0ccd-4b08-8d0d-80370cedda84/.default";
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }

        internal InternalClient Client => this.CreateClient();

        internal HttpClient HttpClient { get; }

        private static readonly object ClientLocker = new object();

        internal InternalClient CreateClient()
        {
            lock (ClientLocker)
            {
                if (this._internalClient != null)
                {
                    return this._internalClient;
                }

                var tokenProvider = this._tokenProviderFactory.GetProvider(this.HttpClient);

                this._internalClient = new InternalClient(new TokenCredentials(tokenProvider))
                {
                    BaseUri = this._options.DocumentGenerationServiceUri,
                };

                return this._internalClient;
            }
        }

        protected Guid ResolveSubscriptionId(Guid? subscriptionId)
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
