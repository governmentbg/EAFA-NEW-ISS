using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class ShipRegisterYearlyQuotaDTO
    {
        public int CatchQuotaId { get; set; }

        public decimal QuotaKg { get; set; }

        public decimal TotalCatch { get; set; }

        public decimal LeftoverQuotaKg { get; set; }

        public List<ShipRegisterQuotaHistoryDTO> QuotaHistory { get; set; }

        public List<ShipRegisterCatchHistoryDTO> CatchHistory { get; set; }
    }
}
