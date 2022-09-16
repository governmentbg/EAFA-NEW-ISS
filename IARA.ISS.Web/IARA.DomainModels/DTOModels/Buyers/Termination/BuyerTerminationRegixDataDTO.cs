using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;

namespace IARA.DomainModels.DTOModels.Buyers.Termination
{
    public class BuyerTerminationRegixDataDTO : BuyerTerminationBaseRegixDataDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedByRegixDataDTO SubmittedBy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForRegixDataDTO SubmittedFor { get; set; }
    }
}
