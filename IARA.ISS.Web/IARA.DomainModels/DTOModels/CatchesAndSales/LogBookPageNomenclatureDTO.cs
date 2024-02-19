using System;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class LogBookPageNomenclatureDTO : NomenclatureDTO
    {
        public string ProductInfo { get; set; }

        public DateTime? FillDate { get; set; }

        public decimal? PageNumber { get; set; }
    }
}
