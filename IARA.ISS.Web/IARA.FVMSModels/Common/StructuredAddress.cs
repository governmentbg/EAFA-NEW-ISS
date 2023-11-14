using System.Text.Json.Serialization;

namespace IARA.FVMSModels.ExternalModels
{
    public class StructuredAddress
    {
        /// <summary>
        /// Код на държавата - пример BGR
        /// </summary>
        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; }

        /// <summary>
        /// Име на града
        /// </summary>
        [JsonPropertyName("city_name")]
        public string CityName { get; set; }

        /// <summary>
        /// Улица (име на улица, номер и т.н.)
        /// </summary>
        [JsonPropertyName("street")]
        public string Street { get; set; }

        public StructuredAddress()
        {
        }

        public StructuredAddress(StructuredAddress rhs)
        {
            CountryCode = rhs.CountryCode;
            CityName = rhs.CityName;
            Street = rhs.Street;
        }
    }
}
