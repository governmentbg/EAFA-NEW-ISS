using System.Xml.Serialization;

namespace TL.JasperReports.Integration.Models
{
    public class Attachment
    {
        [XmlElement(ElementName = "contentType")]
        public string ContentType { get; set; }

        [XmlElement(ElementName = "fileName")]
        public string FileName { get; set; }
    }
}
