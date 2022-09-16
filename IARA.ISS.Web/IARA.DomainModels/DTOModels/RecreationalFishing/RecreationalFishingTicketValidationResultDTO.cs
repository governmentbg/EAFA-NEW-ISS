namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingTicketValidationResultDTO
    {
        public int? CurrentlyActiveUnder14Tickets { get; set; }
        public bool CannotPurchaseTicket { get; set; } = false;
    }
}
