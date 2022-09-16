using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class CapacityCertificateHistoryApplDTO
    {
        public PageCodeEnum PageCode { get; set; }

        public int ApplicationId { get; set; }

        public DateTime ApplicationDate { get; set; }

        public string Reason { get; set; }

        public int? ShipId { get; set; }

        public string ShipName { get; set; } // Used in UI

        public string TransferredCapacityCertificate { get; set; }

        public int? DuplicateCapacityCertificateId { get; set; }

        public string DuplicateCapacityCertificate { get; set; }
    }
}
