namespace Kmd.Logic.DocumentGeneration.Client.ConfigurationSample
{
    public class SampleSharePointOnlineOptions
    {
        /// <summary>
        /// Gets or sets the ClientId for a SharePointOnline template storage area.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the ClientSecret for a SharePointOnline template storage area.
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the SharePointOnline TenantId.
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Gets or sets the SharePointOnline GroupName.
        /// </summary>
        public string GroupName { get; set; }
    }
}
