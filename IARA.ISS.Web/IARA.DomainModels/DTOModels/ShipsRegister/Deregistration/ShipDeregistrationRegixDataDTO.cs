using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.FishingCapacity;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class ShipDeregistrationRegixDataDTO : ShipDeregistrationBaseRegixDataDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedByRegixDataDTO SubmittedBy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForRegixDataDTO SubmittedFor { get; set; }

        public FishingCapacityFreedActionsRegixDataDTO FreedCapacityAction { get; set; }
    }
}
