using IARA.Mobile.Domain.Interfaces;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API
{
    public class BuyerApiDto : IActive
    {
        public int Id { get; set; }
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
        public bool HasUtility { get; set; }
        public bool HasVehicle { get; set; }
        public string UtilityName { get; set; }
        public PersonnelAddressApiDto UtilityAddress { get; set; }
        public string VehicleNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
