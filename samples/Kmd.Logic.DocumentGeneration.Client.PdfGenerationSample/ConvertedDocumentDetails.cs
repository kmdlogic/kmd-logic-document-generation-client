using System;
using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.PdfGenerationSample
{
    public class ConvertedDocumentDetails : DocumentDetails
    {
        public Uri SourceDocumentUrl { get; set; }

        public DocumentFormat SourceDocumentFormat { get; set; }
    }
}
