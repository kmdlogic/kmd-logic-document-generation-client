using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Kmd.Logic.DocumentGeneration.Client
{
    [Serializable]
    public class DocumentGenerationValidationException : Exception
    {
        public IDictionary<string, IList<string>> ValidationErrors { get; }

        public DocumentGenerationValidationException()
        {
        }

        public DocumentGenerationValidationException(IDictionary<string, IList<string>> validationErrors)
            : base(GenerateMessage(validationErrors))
        {
            this.ValidationErrors = validationErrors;
        }

        public DocumentGenerationValidationException(string message, IDictionary<string, IList<string>> validationErrors)
            : base(message)
        {
            this.ValidationErrors = validationErrors;
        }

        public DocumentGenerationValidationException(string message, IDictionary<string, IList<string>> validationErrors, Exception innerException)
            : base(message, innerException)
        {
            this.ValidationErrors = validationErrors;
        }

        public DocumentGenerationValidationException(IDictionary<string, IList<string>> validationErrors, Exception innerException)
            : base(GenerateMessage(validationErrors), innerException)
        {
        }

        public DocumentGenerationValidationException(string message)
            : base(message)
        {
        }

        public DocumentGenerationValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected DocumentGenerationValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string GenerateMessage(IDictionary<string, IList<string>> validationErrors)
        {
            var message = "Invalid document generation parameters";

            if (validationErrors != null && validationErrors.Count > 0)
            {
                message += "(" + string.Join(";", validationErrors.Select(x => $"{x.Key}: {string.Join(",", x.Value)}")) + ")";
            }

            return message;
        }
    }
}
