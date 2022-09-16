using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Common
{
    public class SuspensionReasonNomenclatureDTO : NomenclatureDTO
    {
        public int SuspensionTypeId { get; set; }

        public short? MonthsDuration { get; set; }
    }
}
