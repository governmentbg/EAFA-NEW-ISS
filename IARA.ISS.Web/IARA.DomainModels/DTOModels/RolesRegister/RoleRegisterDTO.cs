using System;

namespace IARA.DomainModels.DTOModels.RolesRegister
{
    public class RoleRegisterDTO
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public int UsersCount { get; set; }

        public bool HasInternalAccess { get; set; }

        public bool HasPublicAccess { get; set; }

        public bool IsActive { get; set; }
    }
}
