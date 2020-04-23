using System;
using Kmd.Logic.DocumentGeneration.Client.Configuration;
using Kmd.Logic.DocumentGeneration.Client.Types;

namespace Kmd.Logic.DocumentGeneration.Client.ServiceMessages
{
    public class DocumentGenerationProgress
    {
        internal DocumentGenerationProgress()
        {
        }

        public Guid Id { get; internal set; }

        public Guid SubscriptionId { get; internal set; }

        public Guid ConfigurationId { get; internal set; }

        public string TemplateId { get; internal set; }

        public string Language { get; internal set; }

        public DocumentFormat DocumentFormat { get; internal set; }

        public HierarchyPath HierarchyPath { get; internal set; }

        public DocumentGenerationState State { get; internal set; }

        public string CallbackUrl { get; internal set; }

        public bool? Debug { get; internal set; }

        public string FailReason { get; internal set; }
    }
}
