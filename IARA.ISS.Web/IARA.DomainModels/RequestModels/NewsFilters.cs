using System;

namespace IARA.DomainModels.RequestModels
{
    public class NewsFilters : BaseRequestModel
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int[] DistrictsIds { get; set; }
    }
}
