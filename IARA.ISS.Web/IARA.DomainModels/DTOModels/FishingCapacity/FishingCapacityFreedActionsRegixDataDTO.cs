using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class FishingCapacityFreedActionsRegixDataDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public FishingCapacityRemainderActionEnum? Action { get; set; }

        [RequiredIf(nameof(Action), "msgRequired", typeof(ErrorResources), FishingCapacityRemainderActionEnum.Transfer)]
        public List<FishingCapacityHolderRegixDataDTO> Holders { get; set; }
    }
}
