using System;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class ShipRegisterQuotaHistoryDTO
    {
        public DateTime DateOfChange { get; set; }

        public decimal ShipQuotaSize { get; set; }

        public decimal ShipQuotaIncrement { get; set; }

        public string IncrementReason { get; set; }
    }
}
