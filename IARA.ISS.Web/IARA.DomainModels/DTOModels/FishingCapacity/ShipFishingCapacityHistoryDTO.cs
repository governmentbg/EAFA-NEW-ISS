using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class ShipFishingCapacityHistoryDTO
    {
        public int Id { get; set; }

        public PageCodeEnum? PageCode { get; set; }

        public int? ApplicationId { get; set; }

        public int ShipId { get; set; }

        public int ShipUID { get; set; }

        public string ShipCfr { get; set; }

        public decimal GrossTonnage { get; set; }

        public decimal Power { get; set; }

        public decimal GrossTonnageChange { get; set; }

        public decimal PowerChange { get; set; }

        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public string Reason { get; set; }

        public bool IsActive { get; set; }
    }
}
