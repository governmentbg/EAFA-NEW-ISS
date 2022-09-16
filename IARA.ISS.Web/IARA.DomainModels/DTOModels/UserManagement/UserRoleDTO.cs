using System;

namespace IARA.DomainModels.DTOModels.UserManagement
{
    public class UserRoleDTO
    {
        public int Id { get; set; }
        public int RoleId { get; set; }

        public string Name { get; set; }

        public DateTime AccessValidFrom { get; set; }

        public DateTime AccessValidTo { get; set; }

        public bool IsActive { get; set; }
    }
}
