using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.DomainModels.DTOModels.CatchQuotas
{
    public class QuotaHistDTO
    {
        public int Id { get; set; }
        public int QuotaId { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal AddRemoveQuota { get; set; }
        public decimal NewQuotaValueKg { get; set; }
        public decimal UnloadedByCurrentDateKg { get; set; }
        public string Basis { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}
