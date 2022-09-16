using System;

namespace IARA.DomainModels.RequestModels
{
    public class FishingCapacityCertificatesFilters : BaseRequestModel
    {
        public int? CertificateId { get; set; }

        public int? CertificateNum { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public string HolderNames { get; set; }

        public string HolderEgnEik { get; set; }

        public decimal? GrossTonnageFrom { get; set; }

        public decimal? GrossTonnageTo { get; set; }

        public decimal? PowerFrom { get; set; }

        public decimal? PowerTo { get; set; }

        public bool? IsCertificateActive { get; set; }

        public int? PersonId { get; set; }

        public int? LegalId { get; set; }
    }
}
