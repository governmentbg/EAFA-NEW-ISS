using System.Text.Json.Serialization;

namespace IARA.FVMSModels.GeoZones
{
    public class ZoneType
    {
        public string Identifier { get; set; }

        public ZoneTypeEnum Type { get; set; }

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
