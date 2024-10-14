using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Common
{
    public class PortNomenclatureDTO : NomenclatureDTO
    {
        public bool IsDanube { get; set; }

        public bool IsBlackSea { get; set; }
    }
}
