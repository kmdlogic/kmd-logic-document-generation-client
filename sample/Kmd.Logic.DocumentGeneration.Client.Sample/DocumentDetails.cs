using System;
using System.IO;
using Kmd.Logic.DocumentGeneration.Client.Models;
using Newtonsoft.Json.Linq;

namespace Kmd.Logic.DocumentGeneration.Client.Sample
{
    public class DocumentDetails
    {
        public Guid? ConfigurationId { get; set; }

        public string HierarchyPath { get; set; }

        public Template Template { get; set; }

        public JObject MergeData { get; set; }

        public DocumentFormat DocumentFormat { get; set; }

        public string LocalDownloadPath { get; set; }

        public AnchorDetails AnchorDetails { get; set; }
    }
}
