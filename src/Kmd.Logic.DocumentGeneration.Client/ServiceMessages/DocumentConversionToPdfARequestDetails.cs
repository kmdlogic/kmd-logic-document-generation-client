﻿using System;
using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.ServiceMessages
{
    public class DocumentConversionToPdfARequestDetails : DocumentConversionRequestDetails
    {
        public DocumentConversionToPdfARequestDetails(
            Uri sourceDocumentUrl,
            DocumentFormat sourceDocumentFormat,
            Uri callbackUrl = null,
            bool debug = false)
            : base(
                sourceDocumentUrl,
                sourceDocumentFormat,
                DocumentFormat.Pdf,
                PdfFormat.PdfA3U,
                callbackUrl,
                debug)
        {
        }
    }
}
