using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace TL.JasperReports.Integration.Models
{
    internal class Cancellation
    {
        [XmlElement(ElementName = "status")]
        [JsonPropertyName("value")]
        public string Status { get; } = "cancelled";
    }
}
