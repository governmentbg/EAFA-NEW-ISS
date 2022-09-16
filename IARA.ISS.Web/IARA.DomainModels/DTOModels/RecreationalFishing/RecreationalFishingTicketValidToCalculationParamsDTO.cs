using System;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingTicketValidToCalculationParamsDTO
    {
        public int TypeId { get; set; }
        public int PeriodId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? TelkValidTo { get; set; }
    }
}
