using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingTicketApplicationDTO
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string TicketNum { get; set; }
        public string TicketHolderName { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public int TicketTypeId { get; set; }
        public string TicketType { get; set; }
        public int TicketPeriodId { get; set; }
        public string TicketPeriod { get; set; }
        public decimal TicketPrice { get; set; }
        public string TicketIssuedBy { get; set; }
        public string TicketStatusName { get; set; }
        public ApplicationStatusesEnum ApplicationStatus { get; set; }
        public bool IsActive { get; set; }
    }
}
