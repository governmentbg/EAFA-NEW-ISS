using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingAddTicketsResultDTO
    {
        public int? PaidTicketApplicationId { get; set; }

        public List<int> TicketIds { get; set; }

        public List<int> ChildTicketIds { get; set; }
    }
}
