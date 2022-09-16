using IARA.Mobile.Insp.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Inspections
{
    public class Buyer : IAddressEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }

        public bool HasVehicle { get; set; }
        public string VehicleNumber { get; set; }

        public bool HasUtility { get; set; }
        public string UtilityName { get; set; }

        public bool HasAddress { get; set; }

        public int? CountryId { get; set; }
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
