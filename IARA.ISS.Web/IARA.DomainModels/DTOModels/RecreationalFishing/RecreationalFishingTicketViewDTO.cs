using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingTicketViewDTO
    {
        public int ID { get; set; }

        public int TypeId { get; set; }

        public string Type { get; set; }

        public int PeriodId { get; set; }

        public string Period { get; set; }

        public string Person { get; set; }

        public DateTime? ValidTo { get; set; }

        public PaymentStatusesEnum PaymentStatus { get; set; }

        public string CancellationReason { get; set; }

        public int DaysRemaining { get; set; }
    }
}
