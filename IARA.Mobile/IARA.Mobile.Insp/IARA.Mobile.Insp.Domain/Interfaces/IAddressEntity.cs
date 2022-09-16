using IARA.Mobile.Domain.Interfaces;

namespace IARA.Mobile.Insp.Domain.Interfaces
{
    public interface IAddressEntity : IEntity
    {
        bool HasAddress { get; set; }
        int? CountryId { get; set; }
        int? DistrictId { get; set; }
        int? MunicipalityId { get; set; }
        int? PopulatedAreaId { get; set; }
        string Region { get; set; }
        string PostalCode { get; set; }
        string Street { get; set; }
        string StreetNum { get; set; }
        string BlockNum { get; set; }
        string EntranceNum { get; set; }
        string FloorNum { get; set; }
        string ApartmentNum { get; set; }
    }
}
