using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationTypeDTO : NomenclatureDTO
    {
        public PageCodeEnum PageCode { get; set; }

        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public bool IsChecked { get; set; }
    }
}
