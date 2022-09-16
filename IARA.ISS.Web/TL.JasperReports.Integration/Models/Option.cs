using System.Text.Json.Serialization;

namespace TL.JasperReports.Integration.Models
{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class Option
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("selected")]
        public bool Selected { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }


}
