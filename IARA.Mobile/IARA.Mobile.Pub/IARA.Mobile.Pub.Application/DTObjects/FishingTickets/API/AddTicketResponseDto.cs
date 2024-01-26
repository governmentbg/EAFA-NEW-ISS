using System.Collections.Generic;

namespace IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API
{
    public class AddTicketResponseDto
    {
        public int? PaidTicketApplicationId { get; set; }
        public string PaidTicketPaymentRequestNum { get; set; }

        public List<int> TicketIds { get; set; }

        public List<int> ChildTicketIds { get; set; }
    }
}
