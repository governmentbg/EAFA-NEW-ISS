using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.CatchQuotas
{
    public class ShipQuotaDTO
    {
        public int Id { get; set; }
        public string AssociationName { get; set; }
        public string ShipName { get; set; }
        public string ShipCFR { get; set; }
        public string ShipExtMarking { get; set; }
        public string Fish { get; set; }
        public int Year { get; set; }
        public decimal QuotaSize { get; set; }
        public decimal UnloadedByCurrentDateKg { get; set; }
        public decimal ConfiscatedQuantity { get; set; }
        public decimal Leftover { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<QuotaHistDTO> ChangeHistoryRecords { get; set; }
    }
}
