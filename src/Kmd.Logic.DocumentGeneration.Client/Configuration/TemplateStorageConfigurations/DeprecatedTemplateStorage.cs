using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.Configuration.TemplateStorageConfigurations
{
    public class DeprecatedTemplateStorage : ITemplateStorageConfiguration
    {
        public TemplateStorageType TemplateStorageType => TemplateStorageType.Deprecated;

        internal DeprecatedTemplateStorage()
        {
        }
    }
}
