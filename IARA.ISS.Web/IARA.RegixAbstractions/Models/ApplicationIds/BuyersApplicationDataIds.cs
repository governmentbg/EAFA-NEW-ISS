namespace IARA.RegixAbstractions.Models.ApplicationIds
{
    public class BuyersApplicationDataIds : BaseRegixApplicationDataIds
    {
        public int? OrganizerPersonId { get; set; }
        public int? AgentPersonId { get; set; }
        public int BuyerId { get; set; }
        public bool HasUtility { get; set; }
    }
}
