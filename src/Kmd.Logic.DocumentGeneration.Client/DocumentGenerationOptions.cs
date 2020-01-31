using System;

namespace Kmd.Logic.DocumentGeneration.Client
{
    /// <summary>
    /// Provide the configuration options for using the documentGeneration service.
    /// </summary>
    public sealed class DocumentGenerationOptions
    {
        /// <summary>
        /// Gets or sets the Logic DocumentGeneration service.
        /// </summary>
        /// <remarks>
        /// This option should not be overridden except for testing purposes.
        /// </remarks>
        public Uri DocumentGenerationServiceUri { get; set; } = new Uri("https://gateway.kmdlogic.io/documentGeneration/v1");

        /// <summary>
        /// Gets or sets the Logic Subscription.
        /// </summary>
        public Guid? SubscriptionId { get; set; }
    }
}
