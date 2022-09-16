using System;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class FishingCapacityCertificateDTO
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }

        public int CertificateNum { get; set; }

        public DateTime CertificateValidFrom { get; set; }

        public DateTime CertificateValidTo { get; set; }

        public string HolderNames { get; set; }

        public decimal GrossTonnage { get; set; }

        public decimal Power { get; set; }

        public bool Invalid { get; set; }

        public int? DeliveryId { get; set; }

        public CapacityCertificateHistoryDTO History { get; set; }

        public bool IsActive { get; set; }
    }
}
