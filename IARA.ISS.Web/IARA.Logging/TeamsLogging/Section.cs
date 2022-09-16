using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace IARA.Logging.TeamsLogging
{

    internal class Section
    {
        private string explicitImage = null;

        [JsonPropertyName("activityTitle")]
        public string ActivityTitle { get; set; }

        [JsonPropertyName("activitySubtitle")]
        public string ActivitySubtitle { get; set; }

        [JsonPropertyName("activityImage")]
        public string ActivityImage
        {
            get
            {
                return explicitImage ?? "https://freeiconshop.com/wp-content/uploads/edd/error-flat.png";
            }
            set
            {
                explicitImage = value;
            }
        }

        [JsonPropertyName("facts")]
        public List<Fact> Facts { get; set; }

        [JsonPropertyName("startGroup")]
        public bool? StartGroup { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
