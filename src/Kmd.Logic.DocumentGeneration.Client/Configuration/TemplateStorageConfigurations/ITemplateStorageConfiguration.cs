using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.Configuration.TemplateStorageConfigurations
{
    public interface ITemplateStorageConfiguration
    {
        TemplateStorageType TemplateStorageType { get; }
    }
}
