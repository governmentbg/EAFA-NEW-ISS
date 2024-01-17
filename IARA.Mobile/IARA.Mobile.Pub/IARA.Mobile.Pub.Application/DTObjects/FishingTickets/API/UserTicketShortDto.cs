using System;

namespace IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API
{
    public class UserTicketShortDto
    {
        public int Id { get; set; }
        public string TypeName { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string PersonFullName { get; set; }
        public string TicketNumber { get; set; }
    }
}
