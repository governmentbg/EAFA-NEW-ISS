using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Application
{
    public class SubmittedByRoleNomenclatureDTO : NomenclatureDTO
    {
        public PageCodeEnum ApplicationPageCode { get; set; }
        public bool HasLetterOfAttorney { get; set; }
    }
}
