using System;

namespace IARA.DomainModels.RequestModels
{
    public class FishingActivityReportsFilters : BaseRequestModel
    {
        public string TripIdentifier { get; set; }

        public int? ShipId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
