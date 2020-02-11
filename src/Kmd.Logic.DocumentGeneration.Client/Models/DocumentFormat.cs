namespace Kmd.Logic.DocumentGeneration.Client.Models
{
    public enum DocumentFormat
    {
        /// <summary>
        /// Saves the document in the plain text format
        /// </summary>
        Txt = 1,

        /// <summary>
        /// Saves the document in the RTF format. All characters above 7-bits are escaped as hexadecimal or Unicode characters
        /// </summary>
        Rtf = 2,

        /// <summary>
        /// Saves the document in the Microsoft Word 97 - 2007 Document format
        /// </summary>
        Doc = 3,

        /// <summary>
        /// Saves the document as an Office Open XML WordprocessingML Document (macro-free).
        /// </summary>
        Docx = 4,

        /// <summary>
        /// Saves the document as PDF (Adobe Portable Document) format
        /// </summary>
        Pdf = 5,
    }
}
