using System;
using System.Runtime.Serialization;

namespace Kmd.Logic.DocumentGeneration.Client.Types
{
    [Serializable]
    public class DocumentGenerationException : Exception
    {
        public string InnerMessage { get; }

        public DocumentGenerationException()
        {
        }

        public DocumentGenerationException(string message)
            : base(message)
        {
        }

        public DocumentGenerationException(string message, string innerMessage)
            : base(message)
        {
            this.InnerMessage = innerMessage;
        }

        public DocumentGenerationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DocumentGenerationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
