using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class ShipRegisterDTO
    {
        public int Id { get; set; }

        public string CFR { get; set; }

        public string ExternalMark { get; set; }

        public string Name { get; set; }

        public string Owners { get; set; }

        public DateTime LastChangeDate { get; set; }

        public string LastChangeStatus { get; set; }

        public ShipEventTypeEnum EventType { get; set; }

        public bool IsThirdPartyShip { get; set; }
    }
}
