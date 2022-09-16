using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TL.JasperReports.Integration.Models
{
    public class InputControlState
    {
        [JsonPropertyName("uri")]
        public string Uri { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("options")]
        public List<Option> Options { get; set; }
    }


}
