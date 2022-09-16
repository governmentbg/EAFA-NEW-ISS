using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API
{
    public class PatrolVehicleApiDto : NomenclatureDto
    {
        public string RegistrationNumber { get; set; }
        public int? InstitutionId { get; set; }
        public int? FlagId { get; set; }
        public int? PatrolVehicleTypeId { get; set; }
        public PatrolVehicleType VehicleType { get; set; }
    }
}
