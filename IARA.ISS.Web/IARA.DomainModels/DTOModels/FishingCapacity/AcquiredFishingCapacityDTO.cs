using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class AcquiredFishingCapacityDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public AcquiredCapacityMannerEnum? AcquiredManner { get; set; }

        [RequiredIf(nameof(AcquiredManner), "msgRequired", typeof(ErrorResources), AcquiredCapacityMannerEnum.Ranking)]
        public decimal? GrossTonnage { get; set; }

        [RequiredIf(nameof(AcquiredManner), "msgRequired", typeof(ErrorResources), AcquiredCapacityMannerEnum.Ranking)]
        public decimal? Power { get; set; }

        [RequiredIf(nameof(AcquiredManner), "msgRequired", typeof(ErrorResources), AcquiredCapacityMannerEnum.FreeCapLicence)]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<int> CapacityLicenceIds { get; set; }
    }
}
