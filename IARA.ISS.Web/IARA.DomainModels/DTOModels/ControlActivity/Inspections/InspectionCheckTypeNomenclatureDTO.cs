using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionCheckTypeNomenclatureDTO : HasDescrNomenclatureDTO
    {
        public int InspectionTypeId { get; set; }
        public bool IsMandatory { get; set; }
        public InspectionCheckTypesEnum CheckType { get; set; }
        public string DescriptionLabel { get; set; }
    }
}
