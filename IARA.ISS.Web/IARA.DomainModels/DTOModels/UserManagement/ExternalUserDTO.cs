using System.Collections.Generic;
using IARA.DomainModels.DTOModels.User;

namespace IARA.DomainModels.DTOModels.UserManagement
{
    public class ExternalUserDTO : UserEditDTO
    {
        public List<UserLegalDTO> UserLegals { get; set; }
    }
}
