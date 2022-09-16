using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.Duplicates
{
    public class DuplicatesEntryDTO
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public DateTime Date { get; set; }

        public string SubmittedBy { get; set; }

        public decimal? Price { get; set; }

        public PageCodeEnum PageCode { get; set; }

        public int? DeliveryId { get; set; }
    }
}
