using System.Xml.Serialization;

namespace TL.JasperReports.Integration.Models
{
    public class Options
    {
        [XmlElement(ElementName = "outputFormat")]
        public string OutputFormat { get; set; }

        [XmlElement(ElementName = "attachmentsPrefix")]
        public string AttachmentsPrefix { get; set; }

        [XmlElement(ElementName = "allowInlineScripts")]
        public bool AllowInlineScripts { get; set; }

        [XmlElement(ElementName = "baseUrl")]
        public string BaseUrl { get; set; }
    }
}
