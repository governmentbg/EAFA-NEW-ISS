using System;

namespace IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API
{
    public class TicketValidationDTO
    {
        public string TypeCode { get; set; }
        public string PeriodCode { get; set; }
        public DateTime? ValidFrom { get; set; }
        public string EgnLnch { get; set; }

        //Under14
        public DateTime? ChildDateOfBirth { get; set; }

        //Disability
        public bool TELKIsIndefinite { get; set; }
        public DateTime? TELKValidTo { get; set; }
    }
}
