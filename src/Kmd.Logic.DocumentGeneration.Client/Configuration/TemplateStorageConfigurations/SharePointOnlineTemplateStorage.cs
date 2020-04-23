using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.Configuration.TemplateStorageConfigurations
{
    public class SharePointOnlineTemplateStorage : ITemplateStorageConfiguration
    {
        public TemplateStorageType TemplateStorageType => TemplateStorageType.SharePointOnline;

        public string ClientId { get; set; }

        public string TenantId { get; set; }

        public string SecretKeyOrClientSecret { get; set; }

        public string GroupName { get; set; }

        public SharePointOnlineTemplateStorage(
            string clientId,
            string tenantId,
            string secretKeyOrClientSecret,
            string groupName)
        {
            this.ClientId = clientId;
            this.TenantId = tenantId;
            this.SecretKeyOrClientSecret = secretKeyOrClientSecret;
            this.GroupName = groupName;
        }

        public SharePointOnlineTemplateStorage(Models.SharePointOnlineTemplateStorageConfiguration sharePointOnlineTemplateStorageConfiguration)
        {
            this.ClientId = sharePointOnlineTemplateStorageConfiguration.ClientId;
            this.TenantId = sharePointOnlineTemplateStorageConfiguration.TenantId;
            this.SecretKeyOrClientSecret = sharePointOnlineTemplateStorageConfiguration.SecretKeyForClientSecret;
            this.GroupName = sharePointOnlineTemplateStorageConfiguration.GroupName;
        }
    }
}
