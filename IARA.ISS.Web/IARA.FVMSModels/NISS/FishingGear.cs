using System.Text.Json.Serialization;

namespace IARA.FVMSModels.ExternalModels
{
    public class FishingGear
    {
        /// <summary>
        /// Vessel CFR
        /// </summary>
        [JsonPropertyName("VCFR")]
        public string CFR { get; set; }

        /// <summary>
        /// ISS FGear quantity
        /// </summary>
        [JsonPropertyName("qty")]
        public int Quantity { get; set; }

        /// <summary>
        /// ISS Fgear note
        /// </summary>
        [JsonPropertyName("zabelejki")]
        public string Notes { get; set; }

        /// <summary>
        /// ISS Fgear eye size
        /// </summary>
        [JsonPropertyName("oko")]
        public double Eye { get; set; }

        /// <summary>
        /// ISS Fgear mark
        /// </summary>
        [JsonPropertyName("marki")]
        public string Marks { get; set; }

        /// <summary>
        /// ISS Fgear description code - OTM
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; set; }

        /// <summary>
        /// ISS Fgear description - Пелагични тралове
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
