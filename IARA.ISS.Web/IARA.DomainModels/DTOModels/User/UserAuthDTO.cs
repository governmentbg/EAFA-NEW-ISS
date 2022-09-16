using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;
using IARA.Security.Enum;

namespace IARA.DomainModels.DTOModels.User
{
    public class UserAuthDTO
    {
        public int ID { get; set; }

        public int PersonId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public EgnLncDTO EgnLnc { get; set; }


        [StringLength(100, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string FirstName { get; set; }

        [StringLength(100, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Username { get; set; }

        [StringLength(100, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string MiddleName { get; set; }

        [StringLength(100, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? HasUserPassLogin { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? HasEAuthLogin { get; set; }

        public bool UserMustChangePassword { get; set; }

        public bool UserMustChangeData { get; set; }

        public LoginTypesEnum CurrentLoginType { get; set; }

        public bool IsInternalUser { get; set; }

        public List<string> Permissions { get; set; }
    }
}
