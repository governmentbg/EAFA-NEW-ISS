using System;

namespace IARA.DomainModels.RequestModels
{
    public class ScientificFishingPublicFilters : BaseRequestModel
    {
        public string RequestNumber { get; set; }
        public string PermitNumber { get; set; }
        public DateTime? CreationDateFrom { get; set; }
        public DateTime? CreationDateTo { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }
}
