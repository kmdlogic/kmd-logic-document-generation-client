using System;

namespace Kmd.Logic.DocumentGeneration.Client.ServiceMessages
{
    public class DocumentGenerationUri
    {
        internal DocumentGenerationUri()
        {
        }

        public Uri Uri { get; internal set; }

        public System.DateTime UriExpiryTime { get; internal set; }
    }
}
