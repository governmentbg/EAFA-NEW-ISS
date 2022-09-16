using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class AquacultureLogBookPageRegisterDTO
    {
        public int Id { get; set; }

        public int LogBookId { get; set; }

        public decimal PageNumber { get; set; }

        public bool IsLogBookFinished { get; set; }

        public DateTime? FillingDate { get; set; }

        public string BuyerName { get; set; }

        public string ProductionInformation { get; set; }

        public LogBookPageStatusesEnum Status { get; set; }

        public string StatusName { get; set; } // For UI

        public string CancellationReason { get; set; }

        public bool IsActive { get; set; }
    }
}
