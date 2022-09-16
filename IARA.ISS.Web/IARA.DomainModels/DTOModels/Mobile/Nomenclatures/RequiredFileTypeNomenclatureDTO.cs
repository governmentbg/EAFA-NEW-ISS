using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Mobile.Nomenclatures
{
    public class RequiredFileTypeNomenclatureDTO : NomenclatureDTO
    {
        public int FileTypeId { get; set; }
        public bool IsMandatory { get; set; }
    }
}
