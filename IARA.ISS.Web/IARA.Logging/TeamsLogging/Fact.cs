using System.Text.Json.Serialization;

namespace IARA.Logging.TeamsLogging
{

    internal class Fact
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }
}
