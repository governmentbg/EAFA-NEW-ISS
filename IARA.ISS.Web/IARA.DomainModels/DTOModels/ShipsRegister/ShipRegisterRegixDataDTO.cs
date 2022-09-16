using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.FishingCapacity;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class ShipRegisterRegixDataDTO : ShipRegisterBaseRegixDataDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedByRegixDataDTO SubmittedBy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForRegixDataDTO SubmittedFor { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<ShipOwnerRegixDataDTO> Owners { get; set; }

        public FishingCapacityFreedActionsRegixDataDTO RemainingCapacityAction { get; set; }

        public List<ApplicationRegiXCheckDTO> ApplicationRegiXChecks { get; set; }
    }
}
