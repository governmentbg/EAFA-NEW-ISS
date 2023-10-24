using System;
using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.FishingActivityReports
{
    public class FishingActivityReportDTO
    {
        public string TripIdentifier { get; set; }

        public string ShipCfr { get; set; }

        public string ShipName { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? UnloadTime { get; set; }

        public List<FishingActivityReportItemDTO> Items { get; set; }

        public List<FishingActivityReportPageDTO> Pages { get; set; }
    }
}
