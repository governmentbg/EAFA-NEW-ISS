using System.Text.Json.Serialization;

namespace IARA.FVMSModels
{
    public class AccessType
    {
        public string Identifier { get; set; }

        [JsonPropertyName("Lvl")]
        public int Level { get; set; }

        [JsonPropertyName("DescBg")]
        public string DescriptionBG { get; set; }

        [JsonPropertyName("DescEng")]
        public string DescriptionEN { get; set; }

        /// <summary>
        /// Код за описание
        /// </summary>
        public int? Code { get; set; }
    }
}
