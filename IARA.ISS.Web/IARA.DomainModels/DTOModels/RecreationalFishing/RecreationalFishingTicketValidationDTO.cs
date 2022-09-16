using System;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingTicketValidationDTO
    {
        public EgnLncDTO PersonEgnLnc { get; set; }
        public bool IsRepresentative { get; set; }
        public int TypeId { get; set; }
        public int PeriodId { get; set; }
        public DateTime ValidFrom { get; set; }
        public int Under14TicketsCount { get; set; }
    }
}
