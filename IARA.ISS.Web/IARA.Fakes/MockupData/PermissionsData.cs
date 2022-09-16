using System;
using System.Collections.Generic;
using IARA.EntityModels.Entities;

namespace IARA.Fakes.MockupData
{
    public class PermissionsData
    {
        public static List<NpermissionType> NpermissionTypes
        {
            get
            {
                return new List<NpermissionType>
                {
                    new NpermissionType { Id = 1, Code = "READ", Name = "Преглед" },
                    new NpermissionType { Id = 2, Code = "ADD", Name = "Добавяне" },
                    new NpermissionType { Id = 3, Code = "EDIT", Name = "Редактиране" },
                    new NpermissionType { Id = 4, Code = "DELETE", Name = "Изтриване" },
                    new NpermissionType { Id = 5, Code = "RESTORE", Name = "Възстановяване" },
                    new NpermissionType { Id = 6, Code = "OTHER", Name = "Друг" }
                };
            }
        }

        public static List<NpermissionGroup> NpermissionGroups
        {
            get
            {
                return new List<NpermissionGroup>
                {
                    new NpermissionGroup { Id = 1, Name = "Даляни", OrderNo = 1 },
                    new NpermissionGroup { Id = 2, Name = "Правоспособни рибари", OrderNo = 2 },
                    new NpermissionGroup { Id = 3, Name = "Научен риболов", OrderNo = 3 },
                    new NpermissionGroup { Id = 4, Name = "Вътрешни потребители", OrderNo = 4 }
                };
            }
        }

        public static List<Npermission> Npermissions
        {
            get
            {
                return new List<Npermission>
                {
                    new Npermission
                    {
                        Id = 1,
                        Name = "PoundnetsRead",
                        Description = "Преглед на даляни",
                        PermissionTypeId = NpermissionTypes[0].Id,
                        PermissionGroupId = NpermissionGroups[0].Id,
                        ValidFrom = DateTime.MinValue,
                        ValidTo = DateTime.MaxValue
                    },
                    new Npermission
                    {
                        Id = 2,
                        Name = "PoundnetsEditRecords",
                        Description = "Редактиране на даляни",
                        PermissionTypeId = NpermissionTypes[2].Id,
                        PermissionGroupId = NpermissionGroups[0].Id,
                        ValidFrom = DateTime.MinValue,
                        ValidTo = DateTime.MaxValue
                    },
                    new Npermission
                    {
                        Id = 3,
                        Name = "ScientificFishingDeleteRecords",
                        Description = "Изтриване на разрешителни за научен риболов",
                        PermissionTypeId = NpermissionTypes[3].Id,
                        PermissionGroupId = NpermissionGroups[2].Id,
                        ValidFrom = DateTime.MinValue,
                        ValidTo = DateTime.MaxValue
                    },
                    new Npermission
                    {
                        Id = 4,
                        Name = "ScientificFishingAddOutings",
                        Description = "Добавяне на излети към разрешително за научен риболов",
                        PermissionTypeId = NpermissionTypes[5].Id,
                        PermissionGroupId = NpermissionGroups[2].Id,
                        ValidFrom = DateTime.MinValue,
                        ValidTo = DateTime.MaxValue
                    },
                    new Npermission
                    {
                        Id = 5,
                        Name = "QualifiedFishersAddRecords",
                        Description = "Добавяне на правоспособни рибари",
                        PermissionTypeId = NpermissionTypes[1].Id,
                        PermissionGroupId = NpermissionGroups[1].Id,
                        ValidFrom = DateTime.MinValue,
                        ValidTo = DateTime.MaxValue
                    },
                    new Npermission
                    {
                        Id = 6,
                        Name = "InternalUsersRestoreRecords",
                        Description = "Възстановяване на вътрешни потребители",
                        PermissionTypeId = NpermissionTypes[4].Id,
                        PermissionGroupId = NpermissionGroups[3].Id,
                        ValidFrom = DateTime.MinValue,
                        ValidTo = DateTime.MaxValue
                    }
                };
            }
        }
    }
}
