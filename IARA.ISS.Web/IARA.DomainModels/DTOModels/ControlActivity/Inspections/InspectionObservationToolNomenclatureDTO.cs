using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionObservationToolNomenclatureDTO : NomenclatureDTO
    {
        public ObservationToolOnBoardEnum OnBoard { get; set; }
    }
}
