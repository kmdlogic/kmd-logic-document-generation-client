using System;
using System.Runtime.Serialization;

namespace Kmd.Logic.DocumentGeneration.Client.Types
{
    [Serializable]
    public class DocumentGenerationConfigurationException : Exception
    {
        public string InnerMessage { get; }

        public DocumentGenerationConfigurationException()
        {
        }

        public DocumentGenerationConfigurationException(string message)
            : base(message)
        {
        }

        public DocumentGenerationConfigurationException(string message, string innerMessage)
            : base(message)
        {
            this.InnerMessage = innerMessage;
        }

        public DocumentGenerationConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DocumentGenerationConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
