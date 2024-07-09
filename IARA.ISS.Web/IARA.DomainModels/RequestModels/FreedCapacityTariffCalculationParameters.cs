using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.FishingCapacity;

namespace IARA.DomainModels.RequestModels
{
    public class FreedCapacityTariffCalculationParameters
    {
        public int? ApplicationId { get; set; }

        public bool? HasFishingCapacity { get; set; }

        public FishingCapacityRemainderActionEnum? Action { get; set; }

        public List<FishingCapacityHolderDTO> Holders { get; set; }

        public List<int> ExcludedTariffsIds { get; set; }
    }
}
