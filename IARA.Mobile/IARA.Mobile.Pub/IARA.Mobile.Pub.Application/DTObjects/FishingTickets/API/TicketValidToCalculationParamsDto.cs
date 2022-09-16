using System;

namespace IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API
{
    public class TicketValidToCalculationParamsDto
    {
        public int TypeId { get; set; }
        public int PeriodId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? TelkValidTo { get; set; }
    }
}
