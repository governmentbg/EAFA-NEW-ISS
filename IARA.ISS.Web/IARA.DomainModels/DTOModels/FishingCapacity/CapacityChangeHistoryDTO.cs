using System;
using System.Collections.Generic;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class CapacityChangeHistoryDTO
    {
        public int Id { get; set; }

        public int? ApplicationId { get; set; }

        public DateTime DateOfChange { get; set; }

        public FishingCapacityChangeTypeEnum TypeOfChange { get; set; }

        public int? ShipCapacityId { get; set; }

        public int? AcquiredFishingCapacityId { get; set; }

        public int? CapacityCertificateTransferId { get; set; }

        public decimal? GrossTonnageChange { get; set; }

        public decimal? PowerChange { get; set; }

        public string ReasonOfChange { get; set; }

        public List<int> CapacityCertificateIds { get; set; }

        public bool IsActive { get; set; }
    }
}
