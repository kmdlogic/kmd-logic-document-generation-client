using System;

namespace Kmd.Logic.DocumentGeneration.Client.PdfGenerationSample
{
    public class GenerationSampleOptions
    {
        // The preconfigured Document Generation Configuration Id
        public Guid? ConfigurationId { get; set; }

        /// <summary>
        /// Gets or sets the hierarchy path to the target template storage configuration.
        /// </summary>
        public string HierarchyPath { get; set; } = @"\";

        /// <summary>
        /// Gets or sets the template subject.
        /// </summary>
        public string Subject { get; set; } = string.Empty;
    }
}
