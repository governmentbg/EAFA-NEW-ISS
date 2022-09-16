using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Common
{
    public class TariffNomenclatureDTO : NomenclatureDTO
    {
        public string BasedOnPlea { get; set; }

        public bool IsCalculated { get; set; }

        public decimal Price { get; set; }
    }
}
