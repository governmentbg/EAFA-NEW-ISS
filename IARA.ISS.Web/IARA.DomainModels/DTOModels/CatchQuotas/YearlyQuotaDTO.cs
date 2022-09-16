using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.CatchQuotas
{
    public class YearlyQuotaDTO
    {
        public int Id { get; set; }
        public string Fish { get; set; }
        public int Year { get; set; }
        public decimal QuotaValueKg { get; set; }
        public decimal UnloadedQuantity { get; set; }
        public decimal Leftover { get; set; }
        public decimal ConfiscatedQuantity { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<QuotaHistDTO> ChangeHistoryRecords { get; set; }
    }
}
