using System;
using Kmd.Logic.DocumentGeneration.Client.Types;
using Newtonsoft.Json.Linq;

namespace Kmd.Logic.DocumentGeneration.Client.ServiceMessages
{
    public class DocumentGenerationRequestDetails
    {
        public DocumentGenerationRequestDetails(
            string hierarchyPath,
            string templateId,
            string twoLetterIsoLanguageName,
            DocumentFormat documentFormat,
            JObject mergeData,
            Uri callbackUrl,
            bool debug)
        {
            this.HierarchyPath = hierarchyPath;
            this.TemplateId = templateId;
            this.TwoLetterIsoLanguageName = twoLetterIsoLanguageName;
            this.DocumentFormat = documentFormat;
            this.MergeData = mergeData;
            this.CallbackUrl = callbackUrl;
            this.Debug = debug;
        }

        /// <summary>
        /// Gets The hierarchy of possible template sources not including the master location.
        /// For example, if you have a customer "A0001" with a department "B0001" then the hierarchy path would be @"\A0001\B0001".
        /// If the department has no template source configured then the customers templates will be used.
        /// </summary>
        public string HierarchyPath { get; }

        /// <summary>
        /// Gets the identifier of template to be used.
        /// </summary>
        public string TemplateId { get; }

        /// <summary>
        /// Gets the Language code in ISO 639-2 format (eg. en, da).
        /// </summary>
        public string TwoLetterIsoLanguageName { get; }

        /// <summary>
        /// Gets the format of the generated document. Possible values include: 'Txt', 'Rtf', 'Doc', 'Docx', 'Pdf'.
        /// </summary>
        public DocumentFormat DocumentFormat { get; }

        /// <summary>
        /// Gets data to be merged into the document.
        /// </summary>
        public JObject MergeData { get; }

        /// <summary>
        /// Gets URL that is going to be called when document generation completes.
        /// </summary>
        public Uri CallbackUrl { get; }

        /// <summary>
        /// Gets a value indicating whether document generation should be run in diagnostic mode.
        /// </summary>
        public bool Debug { get; }
    }
}
