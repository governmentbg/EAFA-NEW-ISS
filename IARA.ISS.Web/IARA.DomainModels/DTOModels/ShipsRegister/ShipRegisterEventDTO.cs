using System;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class ShipRegisterEventDTO
    {
        public int No { get; set; }

        public int ShipId { get; set; }

        public string Type { get; set; }

        public DateTime Date { get; set; }

        public bool UsrRsr { get; set; }

        public bool IsTemporary { get; set; }
    }
}
