using System;
using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.Common
{
    public class FishNomenclatureDTO : NomenclatureDTO
    {
        public int? QuotaId { get; set; }

        public DateTime? QuotaPeriodFrom { get; set; }

        public DateTime? QuotaPeriodTo { get; set; }

        public List<QuotaSpiciesPortDTO> QuotaSpiciesPermittedPortIds { get; set; }

        public FishFamilyTypesEnum? FamilyType { get; set; }

        public bool IsDanube { get; set; }

        public bool IsBlackSea { get; set; }

        public bool IsInternal { get; set; }
    }
}
