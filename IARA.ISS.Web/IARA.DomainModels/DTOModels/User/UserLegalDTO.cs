using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.User
{
    public class UserLegalDTO
    {
        public int LegalId { get; set; }

        public string Name { get; set; }

        public string EIK { get; set; }

        public int RoleId { get; set; }

        public string Role { get; set; }

        public UserLegalStatusEnum Status { get; set; }

        public string StatusName { get; set; }

        public bool IsActive { get; set; }
    }
}
