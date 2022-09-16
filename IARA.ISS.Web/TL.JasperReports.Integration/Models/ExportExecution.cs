using System.Collections.Generic;
using System.Xml.Serialization;

namespace TL.JasperReports.Integration.Models
{
    [XmlRoot(ElementName = "export")]
    public class ExportExecution
    {
        [XmlElement(ElementName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "status")]
        public string Status { get; set; }

        [XmlElement(ElementName = "options")]
        public Options Options { get; set; }

        [XmlElement(ElementName = "outputResource")]
        public OutputResource OutputResource { get; set; }

        [XmlArray(ElementName = "attachments")]
        [XmlArrayItem(ElementName = "attachment")]
        public List<Attachment> Attachments { get; set; }
    }
}
