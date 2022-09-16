using System;
using IARA.DomainModels.DTOModels.CatchesAndSales;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class ShipRegisterOriginDeclarationFishDTO : OriginDeclarationFishDTO
    {
        public string LogBookNum { get; set; }

        public string LogBookPageNum { get; set; }

        public DateTime? Date { get; set; }

        public string FishingGearTypeName { get; set; }

        public int UnloadTypeId { get; set; }

        public int? UnloadPortId { get; set; }

        public DateTime? UnloadDateTime { get; set; }
    }
}
