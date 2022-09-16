using System;

namespace IARA.RegixAbstractions.Models.ApplicationIds
{
    public class TicketApplicationDataIds : BaseRegixApplicationDataIds
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int? PersonRepresentativeId { get; set; }
        public bool TelkIsIndefinite { get; set; }
        public string TelkNumber { get; set; }
        public DateTime? TelkValidTo { get; set; }
    }
}
