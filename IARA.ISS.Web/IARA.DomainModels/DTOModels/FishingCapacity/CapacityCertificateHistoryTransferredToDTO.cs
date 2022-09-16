using System;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class CapacityCertificateHistoryTransferredToDTO
    {
        public int Id { get; set; }

        public string CertificateNum { get; set; }

        public string Holder { get; set; }

        public decimal Tonnage { get; set; }

        public decimal Power { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public bool Invalid { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
