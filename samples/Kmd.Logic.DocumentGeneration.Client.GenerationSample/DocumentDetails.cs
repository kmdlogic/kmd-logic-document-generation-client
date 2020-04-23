using System;
using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.GenerationSample
{
    public class DocumentDetails
    {
        public Guid ConfigurationId { get; set; }

        public DocumentFormat DocumentFormat { get; set; }

        public string Description { get; set; }

        public string LocalDownloadPath { get; set; }

        public AnchorDetails AnchorDetails { get; set; }

        public long FileSize { get; set; }
    }
}
