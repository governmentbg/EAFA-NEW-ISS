using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.User
{
    public class UserTokenDTO
    {
        [StringLength(100, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Token { get; set; }
    }
}
