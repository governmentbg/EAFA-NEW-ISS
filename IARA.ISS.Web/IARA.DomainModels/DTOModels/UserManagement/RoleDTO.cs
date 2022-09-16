using System;

namespace IARA.DomainModels.DTOModels.UserManagement
{
    public class RoleDTO
    {
        public int Id { get; set; }

        public int UserRoleId { get; set; }

        public string Name { get; set; }

        public DateTime AccessValidFrom { get; set; }

        public DateTime AccessValidTo { get; set; }

        public bool IsActive { get; set; }
    }
}
