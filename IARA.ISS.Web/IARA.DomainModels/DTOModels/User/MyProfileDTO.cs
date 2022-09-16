using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.UserManagement;

namespace IARA.DomainModels.DTOModels.User
{
    public class MyProfileDTO : RegixPersonDataDTO
    {
        public int Id { get; set; }//PersonId

        public string Username { get; set; }

        public List<AddressRegistrationDTO> UserAddresses { get; set; }

        public FileInfoDTO Photo { get; set; }

        public List<RoleDTO> Roles { get; set; }

        public List<UserLegalDTO> Legals { get; set; }

        public NewsSubscriptionTypes? NewsSubscription { get; set; }

        public List<UserNewsDistrictSubscriptionDTO> NewsDistrictSubscriptions { get; set; }
    }
}
