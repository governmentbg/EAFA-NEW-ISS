using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingTicketPriceDTO
    {
        public TicketTypeEnum TicketType { get; set; }
        public TicketPeriodEnum TicketPeriod { get; set; }
        public decimal Price { get; set; }
    }
}
