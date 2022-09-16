using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.Duplicates
{
    public class BuyerDuplicateDataDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsOnline { get; set; }

        [RequiredIf(nameof(IsOnline), "msgRequired", typeof(ErrorResources), false)]
        public int? BuyerId { get; set; }

        [RequiredIf(nameof(IsOnline), "msgRequired", typeof(ErrorResources), true)]
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string BuyerUrorrNumber { get; set; }
    }
}
