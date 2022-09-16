using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class PatrolVehicleTypeNomenclatureDTO : NomenclatureDTO
    {
        public PatrolVehicleTypeEnum VehicleType { get; set; }
    }
}
