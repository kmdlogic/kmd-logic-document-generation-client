using Kmd.Logic.DocumentGeneration.Client.ServiceMessages;
using Newtonsoft.Json.Linq;

namespace Kmd.Logic.DocumentGeneration.Client.GenerationSample
{
    public class GeneratedDocumentDetails : DocumentDetails
    {
        public string HierarchyPath { get; set; }

        public DocumentGenerationTemplate Template { get; set; }

        public JObject MergeData { get; set; }
    }
}
