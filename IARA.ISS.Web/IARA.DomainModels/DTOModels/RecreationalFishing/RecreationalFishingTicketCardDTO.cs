using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingTicketCardDTO
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string TicketNum { get; set; }
        public int TypeId { get; set; }
        public string Type { get; set; }
        public int PeriodId { get; set; }
        public string Period { get; set; }
        public string Person { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public decimal Price { get; set; }
        public PaymentStatusesEnum PaymentStatus { get; set; }
        public ApplicationStatusesEnum ApplicationStatus { get; set; }
        public string Status { get; set; }
        public int PercentOfPeriodCompleted { get; set; }
        public bool IsPaymentProcessing { get; set; }
        public bool IsExpiringSoon { get; set; }
        public bool IsExpired { get; set; }
        public bool IsCanceled { get; set; }
    }
}
