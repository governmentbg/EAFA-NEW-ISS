using System;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingTicketDuplicateTableDTO
    {
        public string TicketNum { get; set; }

        public DateTime IssueDate { get; set; }

        public decimal Price { get; set; }

        public bool IsActive { get; set; }
    }
}
