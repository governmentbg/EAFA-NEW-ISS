namespace IARA.Flux.Models
{
    public partial class VesselCountryType
    {
        public static VesselCountryType BuildVesselCountry(string countryCode)
        {
            return new VesselCountryType
            {
                ID = new IDType
                {
                    schemeID = "TERRITORY",
                    Value = countryCode
                }
            };
        }
    }
}
