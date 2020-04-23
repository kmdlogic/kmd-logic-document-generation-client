using System;
using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.ServiceMessages
{
    public class DocumentConversionRequest
    {
        /// <summary>
        /// Gets URL that identifies the document to be converted.
        /// </summary>
        public Uri SourceDocumentUrl { get; }

        /// <summary>
        /// Gets format of the source document.
        /// </summary>
        public DocumentFormat SourceDocumentFormat { get; }

        /// <summary>
        /// Gets format of the converted document.
        /// </summary>
        public DocumentFormat ConvertedDocumentFormat { get; }

        /// <summary>
        /// Gets optional PdfFormat of the converted document.
        /// </summary>
        public PdfFormat? ConvertedDocumentPdfFormat { get; }

        /// <summary>
        /// Gets URL that is going to be called when document generation completes.
        /// </summary>
        public Uri CallbackUrl { get; }

        /// <summary>
        /// Gets a value indicating whether document generation should be run in diagnostic mode.
        /// </summary>
        public bool Debug { get; }

        public DocumentConversionRequest(
            Uri sourceDocumentUrl,
            DocumentFormat sourceDocumentFormat,
            DocumentFormat convertedDocumentFormat,
            PdfFormat? convertedDocumentPdfFormat,
            Uri callbackUrl = null,
            bool debug = false)
        {
            this.SourceDocumentUrl = sourceDocumentUrl;
            this.SourceDocumentFormat = sourceDocumentFormat;
            this.ConvertedDocumentFormat = convertedDocumentFormat;
            this.ConvertedDocumentPdfFormat = convertedDocumentPdfFormat;
            this.CallbackUrl = callbackUrl;
            this.Debug = debug;
        }
    }
}
