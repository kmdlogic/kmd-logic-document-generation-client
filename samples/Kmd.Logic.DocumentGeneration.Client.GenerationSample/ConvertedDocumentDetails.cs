using System;
using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.GenerationSample
{
    public class ConvertedDocumentDetails : DocumentDetails
    {
        public Uri SourceDocumentUrl { get; set; }

        public DocumentFormat SourceDocumentFormat { get; set; }
    }
}
