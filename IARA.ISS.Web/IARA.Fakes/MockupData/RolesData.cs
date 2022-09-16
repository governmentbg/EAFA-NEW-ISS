using System;
using System.Collections.Generic;
using IARA.Common.Constants;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class RolesData
    {
        public static List<Role> Roles
        {
            get
            {
                return new List<Role>
                {
                    new Role
                    {
                        Id = 1,
                        Name = "Управление на потребители",
                        ConcurrencyStamp = "a",
                        HasInternalAccess = true,
                        HasPublicAccess = false,
                        Code = "UP",
                        ValidFrom = DateTime.Now.AddMonths(-5),
                        ValidTo = DateTime.Now.AddDays(-5)
                    },
                    new Role
                    {
                        Id = 2,
                        Name = "Администрация",
                        Description = "Достъп до административни страници",
                        ConcurrencyStamp = "b",
                        HasInternalAccess = true,
                        HasPublicAccess = false,
                        Code = "AD"
                    },
                    new Role
                    {
                        Id = 3,
                        Name = "Титуляр",
                        ConcurrencyStamp = "b",
                        HasInternalAccess = false,
                        HasPublicAccess = true,
                        Code = "T"
                    },
                    new Role
                    {
                        Id = -1, // MUST be -1
                        Code = "PUBLIC_USER",
                        Name = "Публичен потребител",
                        ConcurrencyStamp = "в",
                        HasInternalAccess = false,
                        HasPublicAccess = true,
                        ValidFrom = new DateTime(2021, 4, 26, 12, 27, 22),
                        ValidTo = DefaultConstants.MAX_VALID_DATE
                    },
                    new Role
                    {
                        Id = 4,
                        Name = "Стара роля",
                        ConcurrencyStamp = "b",
                        HasInternalAccess = false,
                        HasPublicAccess = true,
                        Code = "ь",
                        ValidFrom = DateTime.Now.AddMonths(-5),
                        ValidTo = DateTime.Now.AddDays(-5)
                    }
                };
            }
        }

        public static List<RolePermission> RolePermissions
        {
            get
            {
                return new List<RolePermission>
                {
                    new RolePermission { RoleId = Roles[0].Id, PermissionId = PermissionsData.Npermissions[0].Id },
                    new RolePermission { RoleId = Roles[0].Id, PermissionId = PermissionsData.Npermissions[1].Id },
                    new RolePermission { RoleId = Roles[0].Id, PermissionId = PermissionsData.Npermissions[2].Id },
                    new RolePermission { RoleId = Roles[0].Id, PermissionId = PermissionsData.Npermissions[3].Id },
                    new RolePermission { RoleId = Roles[0].Id, PermissionId = PermissionsData.Npermissions[4].Id },
                    new RolePermission { RoleId = Roles[1].Id, PermissionId = PermissionsData.Npermissions[3].Id },
                    new RolePermission { RoleId = Roles[1].Id, PermissionId = PermissionsData.Npermissions[4].Id },
                    new RolePermission { RoleId = Roles[1].Id, PermissionId = PermissionsData.Npermissions[5].Id },
                    new RolePermission { RoleId = Roles[2].Id, PermissionId = PermissionsData.Npermissions[0].Id },
                    new RolePermission { RoleId = Roles[2].Id, PermissionId = PermissionsData.Npermissions[3].Id },
                    new RolePermission { RoleId = Roles[2].Id, PermissionId = PermissionsData.Npermissions[5].Id },
                    new RolePermission { RoleId = Roles[3].Id, PermissionId = PermissionsData.Npermissions[0].Id },
                    new RolePermission { RoleId = Roles[3].Id, PermissionId = PermissionsData.Npermissions[1].Id }
                };
            }
        }
    }
}
