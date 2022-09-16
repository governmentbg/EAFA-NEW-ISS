using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class InspectionCheckTypeDto : DescrSelectNomenclatureDto
    {
        public bool IsMandatory { get; set; }
        public ToggleTypeEnum Type { get; set; }
        public string DescriptionLabel { get; set; }
    }
}
