using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class SailAreaNomenclatureDTO : NomenclatureDTO
    {
        public decimal? MaxShoreDistance { get; set; }

        public decimal? MaxSeaState { get; set; }
    }
}
