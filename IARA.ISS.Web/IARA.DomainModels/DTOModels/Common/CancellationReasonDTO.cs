using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Common
{
    public class CancellationReasonDTO : NomenclatureDTO
    {
        public CancellationReasonGroupEnum Group { get; set; }
    }
}
