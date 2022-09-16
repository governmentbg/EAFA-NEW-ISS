using System;

namespace IARA.DomainModels.RequestModels
{
    public class FluxFlapRequestFilters : BaseRequestModel
    {
        public int? ShipId { get; set; }

        public string ShipIdentifier { get; set; }

        public string ShipName { get; set; }

        public string RequestUuid { get; set; }

        public string ResponseUuid { get; set; }

        public DateTime? RequestDateFrom { get; set; }

        public DateTime? RequestDateTo { get; set; }
    }
}
