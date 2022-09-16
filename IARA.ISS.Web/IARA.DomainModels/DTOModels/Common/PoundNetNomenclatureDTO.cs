using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Common
{
    public class PoundNetNomenclatureDTO : NomenclatureDTO
    {
        public string StatusCode { get; set; }

        public string StatusName { get; set; }

        public bool HasPoundNetPermit { get; set; }

        public string Depth { get; set; }
    }
}
