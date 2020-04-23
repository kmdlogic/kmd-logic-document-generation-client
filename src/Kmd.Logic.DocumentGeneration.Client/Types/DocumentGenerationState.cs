namespace Kmd.Logic.DocumentGeneration.Client.Types
{
    public enum DocumentGenerationState
    {
        /// <summary>
        /// Document generation has been requested
        /// </summary>
        Requested = 1,

        /// <summary>
        /// Document generation has completed and the document is ready for download
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Document generation has failed
        /// </summary>
        Failed = 3,
    }
}
