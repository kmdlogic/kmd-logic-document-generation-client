using System;
using Kmd.Logic.DocumentGeneration.Client.Configuration;
using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.ServiceMessages
{
    public class DocumentGenerationProgress
    {
        public Guid Id { get; set; }

        public Guid SubscriptionId { get; set; }

        public Guid ConfigurationId { get; set; }

        public string TemplateId { get; set; }

        public string Language { get; set; }

        public DocumentFormat DocumentFormat { get; set; }

        public HierarchyPath HierarchyPath { get; set; }

        public DocumentGenerationState State { get; set; }

        public string CallbackUrl { get; set; }

        public bool? Debug { get; set; }

        public string FailReason { get; set; }
    }
}
