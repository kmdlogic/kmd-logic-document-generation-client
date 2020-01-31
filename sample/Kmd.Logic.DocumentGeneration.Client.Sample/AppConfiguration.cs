using System;
using Kmd.Logic.Identity.Authorization;

namespace Kmd.Logic.DocumentGeneration.Client.Sample
{
    internal class AppConfiguration
    {
        public LogicTokenProviderOptions TokenProvider { get; set; } = new LogicTokenProviderOptions();

        public DocumentGenerationOptions DocumentGeneration { get; set; } = new DocumentGenerationOptions();
    }
}
