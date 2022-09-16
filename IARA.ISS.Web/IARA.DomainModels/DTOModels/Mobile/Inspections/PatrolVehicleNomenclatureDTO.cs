using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Mobile.Inspections
{
    public class PatrolVehicleNomenclatureDTO : NomenclatureDTO
    {
        public string RegistrationNumber { get; set; }
        public int? InstitutionId { get; set; }
        public int? FlagId { get; set; }
        public int? PatrolVehicleTypeId { get; set; }
        public PatrolVehicleTypeEnum VehicleType { get; set; }
    }
}
