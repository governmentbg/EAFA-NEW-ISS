using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionVesselActivityNomenclatureDTO : HasDescrNomenclatureDTO
    {
        public bool IsFishingActivity { get; set; }
    }
}
