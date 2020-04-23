namespace Kmd.Logic.DocumentGeneration.Client.ServiceMessages
{
    public class DocumentGenerationTemplate
    {
        internal DocumentGenerationTemplate()
        {
        }

        public string TemplateId { get; set; }

        public string[] Languages { get; set; }
    }
}
