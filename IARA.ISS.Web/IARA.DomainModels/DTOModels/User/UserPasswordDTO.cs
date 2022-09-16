using System.ComponentModel.DataAnnotations;
using IARA.Common.Constants;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.User
{
    public class UserPasswordDTO
    {
        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string OldPassword { get; set; }

        [StringLength(200, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [RegularExpression(DefaultConstants.PASSWORD_COMPLEXITY_PATTERN, ErrorMessageResourceName = "msgPatternNotMatching", ErrorMessageResourceType = typeof(ErrorResources))]
        public string NewPassword { get; set; }
    }
}
