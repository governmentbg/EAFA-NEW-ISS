using System;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class ShipRegisterLogBookPageDTO
    {
        public int Id { get; set; }

        public string LogBookNum { get; set; }

        public string PageNum { get; set; }

        public string FishingGear { get; set; }

        public DateTime OutingStartDate { get; set; }

        public string PortOfUnloading { get; set; }

        public bool IsActive { get; set; }
    }
}
