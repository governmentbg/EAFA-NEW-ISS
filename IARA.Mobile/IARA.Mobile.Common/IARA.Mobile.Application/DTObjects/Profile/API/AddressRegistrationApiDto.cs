using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Application.DTObjects.Profile.API
{
    public class AddressRegistrationApiDto
    {
        public AddressType AddressType { get; set; }
        public int CountryId { get; set; }
        public int? DistrictId { get; set; }
        public int? MunicipalityId { get; set; }
        public int? PopulatedAreaId { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string StreetNum { get; set; }
        public string BlockNum { get; set; }
        public string EntranceNum { get; set; }
        public string FloorNum { get; set; }
        public string ApartmentNum { get; set; }
    }
}
