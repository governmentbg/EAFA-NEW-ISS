using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;
using IARA.Security.Enum;

namespace IARA.DomainModels.DTOModels.User
{
    public class UserRegistrationDTO : UserAuthDTO
    {
        [StringLength(100, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Email { get; set; }

        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [RequiredIf(nameof(CurrentLoginType), "msgRequired", typeof(ErrorResources), LoginTypesEnum.PASSWORD)]
        public string Password { get; set; }
    }
}
