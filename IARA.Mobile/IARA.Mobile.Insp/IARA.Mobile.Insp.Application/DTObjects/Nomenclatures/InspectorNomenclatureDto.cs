using IARA.Mobile.Application.DTObjects.Nomenclatures;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class InspectorNomenclatureDto : SelectNomenclatureDto
    {
        public int? UserId { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
