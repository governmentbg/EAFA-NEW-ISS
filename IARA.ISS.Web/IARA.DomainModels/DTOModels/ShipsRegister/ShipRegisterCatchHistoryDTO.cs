using System;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class ShipRegisterCatchHistoryDTO
    {
        public DateTime DateOfCatch { get; set; }

        public decimal QuantityKg { get; set; }

        public string PlaceOfCatch { get; set; }

        public string LogbookPage { get; set; }
    }
}
