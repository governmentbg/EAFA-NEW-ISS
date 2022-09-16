using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.Buyers.Termination
{
    public abstract class BuyerTerminationBaseRegixDataDTO : BaseRegixChecksDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public override int? ApplicationId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public override PageCodeEnum? PageCode { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsOnline { get; set; }

        [RequiredIf(nameof(IsOnline), "msgRequired", typeof(ErrorResources), false)]
        public int? BuyerId { get; set; }

        [RequiredIf(nameof(IsOnline), "msgRequired", typeof(ErrorResources), true)]
        public string BuyerUrorrNumber { get; set; }
    }
}
