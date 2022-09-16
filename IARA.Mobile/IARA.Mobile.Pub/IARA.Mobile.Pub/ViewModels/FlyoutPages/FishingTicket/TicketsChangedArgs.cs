namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket
{
    public class TicketsChangedArgs
    {
        public const string TICKETS_CHANGED_EVENT = nameof(TICKETS_CHANGED_EVENT);
        public TicketsChangedEnum Change { get; set; }
        public TicketsChangedArgs(TicketsChangedEnum change)
        {
            Change = change;
        }
    }

    public enum TicketsChangedEnum
    {
        NewTicket,
        PaidTicket,
        UpdatedTicket
    }
}
