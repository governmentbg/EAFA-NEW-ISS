using System.Xml.Serialization;

namespace TL.JasperReports.Integration.Models
{
    public class ComplexStatus
    {
        [XmlElement(ElementName = "errorDescriptor")]
        public ErrorDescriptor ErrorDescriptor { get; set; }


        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }
}
