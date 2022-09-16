using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ChangeOfCircumstancesTypeDTO : NomenclatureDTO
    {
        public PageCodeEnum PageCode { get; set; }

        public ChangeOfCircumstancesDataTypeEnum DataType { get; set; }

        public bool IsDeletion { get; set; }
    }
}
