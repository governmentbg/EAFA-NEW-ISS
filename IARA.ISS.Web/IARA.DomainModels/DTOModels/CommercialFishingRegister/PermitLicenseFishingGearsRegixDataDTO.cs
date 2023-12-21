using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using System.ComponentModel.DataAnnotations;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public class PermitLicenseFishingGearsRegixDataDTO : BaseRegixChecksDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedByRegixDataDTO SubmittedBy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public ApplicationSubmittedForRegixDataDTO SubmittedFor { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? PermitLicenseId { get; set; }
    }
}
