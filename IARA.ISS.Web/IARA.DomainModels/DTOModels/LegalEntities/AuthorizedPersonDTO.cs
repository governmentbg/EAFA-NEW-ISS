using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.UserManagement;

namespace IARA.DomainModels.DTOModels.LegalEntities
{
    public class AuthorizedPersonDTO : AuthorizedPersonRegixDataDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<RoleDTO> Roles { get; set; }
    }
}
