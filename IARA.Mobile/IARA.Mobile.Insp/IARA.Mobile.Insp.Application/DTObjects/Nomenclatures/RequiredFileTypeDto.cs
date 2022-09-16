using IARA.Mobile.Application.DTObjects.Nomenclatures;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class RequiredFileTypeDto : NomenclatureDto
    {
        public int FileTypeId { get; set; }
        public bool IsMandatory { get; set; }
    }
}
