using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class AdmissionLogBookPageRegisterDTO
    {
        public int Id { get; set; }

        public int LogBookId { get; set; }

        public decimal PageNumber { get; set; }

        public bool IsLogBookFinished { get; set; }

        public string AcceptedPersonNames { get; set; }

        public DateTime? HandoverDate { get; set; }

        public string StorageLocation { get; set; }

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
