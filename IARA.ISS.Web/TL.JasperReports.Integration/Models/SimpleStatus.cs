using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace TL.JasperReports.Integration.Models
{
    public class SimpleStatus
    {

        [XmlElement(ElementName = "status")]
        [JsonPropertyName("value")]
        public string Status { get; set; }
    }
}
