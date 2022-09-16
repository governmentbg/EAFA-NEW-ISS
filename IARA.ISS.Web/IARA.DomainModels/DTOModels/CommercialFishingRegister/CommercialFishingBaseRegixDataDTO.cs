using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.CommercialFishingRegister
{
    public abstract class CommercialFishingBaseRegixDataDTO : BaseRegixChecksDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public override PageCodeEnum? PageCode { get; set; }

        public ApplicationSubmittedByRegixDataDTO SubmittedBy { get; set; }

        public ApplicationSubmittedForRegixDataDTO SubmittedFor { get; set; }
    }
}
