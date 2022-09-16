
using System.Collections.Generic;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class BasicLogBookPageDocumentDataDTO
    {
        public int ShipLogBookPageId { get; set; }

        public int LogBookId { get; set; }

        public string LogBookNumber { get; set; }

        public LogBookPagePersonTypesEnum OwnerType { get; set; }

        /// <summary>
        /// Data for person is for Admission and Transportation documents only
        /// </summary>
        public LogBookPagePersonDTO PersonData { get; set; }

        /// <summary>
        /// Data for registered buyers is for First sale documents only
        /// </summary>
        public int RegisteredBuyerId { get; set; }

        /// <summary>
        /// Page number to be added (for first sale, admission or transportation log book)
        /// </summary>
        public decimal DocumentNumber { get; set; }

        public LogBookPageStatusesEnum? PageStatus { get; set; }

        public CommonLogBookPageDataDTO SourceData { get; set; }
    }
}
