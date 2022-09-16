using System.Text.Json.Serialization;

namespace IARA.Logging.TeamsLogging
{
    internal class TeamsErrorMessageModel
    {
        private string explicitColor = null;

        [JsonPropertyName("type")]
        public string Type
        {
            get
            {
                return "MessageCard";
            }
        }

        [JsonPropertyName("themeColor")]
        public string ThemeColor
        {
            get
            {
                return explicitColor ?? "FF0000";
            }
            set
            {
                explicitColor = value;
            }
        }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("sections")]
        public Section[] Sections { get; set; }

        [JsonPropertyName("markdown")]
        public bool Markdown
        {
            get
            {
                return true;
            }
        }
    }
}