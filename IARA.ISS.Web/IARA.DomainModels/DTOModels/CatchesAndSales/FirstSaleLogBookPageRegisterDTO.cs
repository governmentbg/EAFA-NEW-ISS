using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class FirstSaleLogBookPageRegisterDTO
    {
        public int Id { get; set; }

        public int LogBookId { get; set; }

        public decimal PageNumber { get; set; }

        public bool IsLogBookFinished { get; set; }

        public string BuyerNames { get; set; }

        public string SaleLocation { get; set; }

        public DateTime? SaleDate { get; set; }

        public string ProductsInformation { get; set; }

        public LogBookPageStatusesEnum Status { get; set; }

        public string StatusName { get; set; } // For UI

        public string CancellationReason { get; set; }

        public bool IsActive { get; set; }
    }
}
