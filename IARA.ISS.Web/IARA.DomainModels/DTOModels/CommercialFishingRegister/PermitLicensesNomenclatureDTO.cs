using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class PermitLicensesNomenclatureDTO : PermitNomenclatureDTO
    {
        public string WaterTypeCode { get; set; }

        public List<string> Tariffs { get; set; }
    }
}
