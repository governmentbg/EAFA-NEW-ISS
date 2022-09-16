using System;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class FishingCapacityCertificateNomenclatureDTO : NomenclatureDTO
    {
        public decimal GrossTonnage { get; set; }

        public decimal Power { get; set; }

        public DateTime ValidTo { get; set; }
    }
}
