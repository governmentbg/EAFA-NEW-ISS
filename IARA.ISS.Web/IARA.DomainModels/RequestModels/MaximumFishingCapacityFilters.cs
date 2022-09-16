using System;

namespace IARA.DomainModels.RequestModels
{
    public class MaximumFishingCapacityFilters : BaseRequestModel
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string Regulation { get; set; }
    }
}
