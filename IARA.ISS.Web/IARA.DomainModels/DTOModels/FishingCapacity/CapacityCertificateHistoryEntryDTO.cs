using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class CapacityCertificateHistoryEntryDTO
    {
        public PageCodeEnum PageCode { get; set; }

        public int ApplicationId { get; set; }

        public DateTime DateOfEvent { get; set; }

        public string Reason { get; set; }

        public bool IsActive { get; set; }
    }
}
