using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class TransportationLogBookPageRegisterDTO
    {
        public int Id { get; set; }

        public int LogBookId { get; set; }

        public decimal PageNumber { get; set; }

        public bool IsLogBookFinished { get; set; }

        public string VehicleNumber { get; set; }

        public string DeliveryLocation { get; set; }

        public string RecieverName { get; set; }

        public DateTime? LoadingDate { get; set; }

        public string ProductsInformation { get; set; }

        public LogBookPageStatusesEnum Status { get; set; }

        /// <summary>
        /// Used for visualization in UI only
        /// </summary>
        public string StatusName { get; set; }

        public bool ConsistsOriginProducts { get; set; }

        public string CancellationReason { get; set; }

        public bool IsActive { get; set; }
    }
}
