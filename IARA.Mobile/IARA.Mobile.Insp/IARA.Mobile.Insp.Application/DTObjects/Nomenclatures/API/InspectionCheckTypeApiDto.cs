using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API
{
    public class InspectionCheckTypeApiDto : DescrNomenclatureDto
    {
        public int InspectionTypeId { get; set; }
        public bool IsMandatory { get; set; }
        public ToggleTypeEnum CheckType { get; set; }
        public string DescriptionLabel { get; set; }
    }
}
