using IARA.Mobile.Pub.Domain.Enums;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket
{
    public class TicketMetadataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BadgeText { get; set; }
        public int TypeId { get; set; }
        public string TypeCode { get; set; }
        public string TypeName { get; set; }
        public int PeriodId { get; set; }
        public string PeriodCode { get; set; }
        public TicketAction Action { get; set; }
        public decimal Price { get; set; }
        public string PaymentRequestNum { get; set; }
        public string ApplicationStatusCode { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public string TicketNumber { get; set; }

    }

    public enum TicketAction
    {
        View,
        Create,
        Update,
        Renew
    }
}
