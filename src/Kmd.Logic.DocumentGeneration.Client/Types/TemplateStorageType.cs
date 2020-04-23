namespace Kmd.Logic.DocumentGeneration.Client.Types
{
    public enum TemplateStorageType
    {
        /// <summary>
        /// Deprecated Template Storage.
        /// This value is deprecated and cannot be used for new storage areas.
        /// </summary>
        Deprecated = 1,

        /// <summary>
        /// SharePoint Online Template Storage
        /// </summary>
        SharePointOnline = 2,

        /// <summary>
        /// Azure Blob Template Storage
        /// </summary>
        AzureBlobStorage = 3,
    }
}
