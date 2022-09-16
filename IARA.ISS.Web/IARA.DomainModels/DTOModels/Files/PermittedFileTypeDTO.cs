using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Files
{
    public class PermittedFileTypeDTO : NomenclatureDTO
    {
        public bool IsRequired { get; set; }

        public PageCodeEnum PageCode { get; set; }
    }
}
