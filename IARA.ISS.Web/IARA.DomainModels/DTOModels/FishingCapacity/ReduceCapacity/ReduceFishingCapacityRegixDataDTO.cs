using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;

namespace IARA.DomainModels.DTOModels.FishingCapacity.ReduceCapacity
{
    public class ReduceFishingCapacityRegixDataDTO : ReduceFishingCapacityBaseRegixDataDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedByRegixDataDTO SubmittedBy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForRegixDataDTO SubmittedFor { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public FishingCapacityFreedActionsRegixDataDTO FreedCapacityAction { get; set; }
    }
}
