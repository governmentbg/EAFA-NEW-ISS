using System;
using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class RecreationalFishingTicketApplicationFilters : BaseRequestModel
    {
        public string TicketNum { get; set; }
        public List<int> TypeIds { get; set; }
        public List<int> PeriodIds { get; set; }
        public string TicketHolderName { get; set; }
        public string TicketHolderEGN { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string TicketIssuerName { get; set; }
        public bool? IsDuplicate { get; set; }
        public List<int> StatusIds { get; set; }
        public bool? ShowOnlyNotFinished { get; set; }
        public int? PersonId { get; set; }
        public int? TerritoryUnitId { get; set; }
    }
}
