using System;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.Common
{
    public class FishQuotaDTO
    {
        public int Id { get; set; }

        public DateTime PeriodFrom { get; set; }

        public DateTime PeriodTo { get; set; }

        public List<QuotaSpiciesPortDTO> PermittedPorts { get; set; }

        public bool IsActive { get; set; }
    }
}
