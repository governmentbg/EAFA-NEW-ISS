using System.Xml.Serialization;

namespace TL.JasperReports.Integration.Models
{
    public class OutputResource
    {
        [XmlElement(ElementName = "contentType")]
        public string ContentType { get; set; }
    }
}
