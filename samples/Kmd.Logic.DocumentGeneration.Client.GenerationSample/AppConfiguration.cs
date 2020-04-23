using Kmd.Logic.Identity.Authorization;

namespace Kmd.Logic.DocumentGeneration.Client.GenerationSample
{
    internal class AppConfiguration
    {
        public LogicTokenProviderOptions TokenProvider { get; set; } = new LogicTokenProviderOptions();

        public DocumentGenerationOptions DocumentGeneration { get; set; } = new DocumentGenerationOptions();

        public GenerationSampleOptions GenerationSample { get; set; } = new GenerationSampleOptions();
    }
}
