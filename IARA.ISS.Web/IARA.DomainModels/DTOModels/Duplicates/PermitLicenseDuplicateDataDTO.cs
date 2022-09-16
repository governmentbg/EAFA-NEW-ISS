using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.Duplicates
{
    public class PermitLicenseDuplicateDataDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsOnline { get; set; }

        [RequiredIf(nameof(IsOnline), "msgRequired", typeof(ErrorResources), false)]
        public int? PermitLicenceId { get; set; }

        [RequiredIf(nameof(IsOnline), "msgRequired", typeof(ErrorResources), false)]
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string PermitLicenceRegistrationNumber { get; set; }
    }
}
