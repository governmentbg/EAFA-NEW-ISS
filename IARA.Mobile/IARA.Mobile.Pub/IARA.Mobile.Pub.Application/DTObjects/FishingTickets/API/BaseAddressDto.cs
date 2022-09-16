using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API
{
    public class BaseAddressDto
    {
        public AddressType Type { get; set; }
        public int CountryId { get; set; }
        public int? DistrictId { get; set; }
        public int? MunicipalityId { get; set; }
        public int? PopulatedAreaId { get; set; }
        public string Address { get; set; }
    }
}
