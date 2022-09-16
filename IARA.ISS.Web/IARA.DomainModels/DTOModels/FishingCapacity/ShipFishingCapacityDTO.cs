using System;
using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class ShipFishingCapacityDTO
    {
        public int Id { get; set; }

        public int ShipId { get; set; }

        public string ShipCfr { get; set; }

        public string ShipName { get; set; }

        public decimal GrossTonnage { get; set; }

        public decimal Power { get; set; }

        public DateTime DateOfChange { get; set; }

        public List<ShipFishingCapacityHistoryDTO> History { get; set; }

        public bool IsActive { get; set; }
    }
}
