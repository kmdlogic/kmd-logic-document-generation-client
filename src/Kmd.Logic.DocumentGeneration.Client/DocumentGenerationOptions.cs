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
        public Uri DocumentGenerationServiceUri { get; set; } = new Uri("https://gateway.kmdlogic.io/document-generation/v2");

        /// <summary>
        /// Gets or sets the Logic Subscription.
        /// </summary>
        public Guid? SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the serializer should ignore null values or empty arrays.
        /// </summary>
        public bool IncludeNullValues { get; set; } = false;
    }
}
