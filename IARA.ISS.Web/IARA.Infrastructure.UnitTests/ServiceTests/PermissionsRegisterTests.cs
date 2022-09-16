using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.PermissionsRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Fakes.MockupData;
using IARA.Interfaces;
using Xunit;

namespace IARA.Infrastructure.UnitTests.ServiceTests
{
    public class PermissionsRegisterTests
    {
        private readonly IARADbContext db;
        private readonly IPermissionsRegisterService service;

        public PermissionsRegisterTests(IARADbContext db, IPermissionsRegisterService service)
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity("Bearer", "TEST_USER", "TEST_ROLE"));
            this.db = db;
            this.service = service;

            SetupDb();
        }

        [Fact(DisplayName = "Извличане на всички права")]
        public void TestGetAllPermissions()
        {
            DateTime now = DateTime.Now;

            PermissionsRegisterFilters filters = new();
            List<PermissionRegisterDTO> records = service.GetAllPermissions(filters).ToList();

            List<Npermission> dbRecords = db.Npermissions.Where(x => x.ValidFrom <= now && x.ValidTo >= now).ToList();
            Assert.Equal(records.Count(), dbRecords.Count());
            foreach (Npermission permission in dbRecords)
            {
                PermissionRegisterDTO result = records.Where(x => x.Id == permission.Id).SingleOrDefault();
                Assert.NotNull(result);

                AssertMatching(permission, result);
            }
        }

        [Fact(DisplayName = "Извличане на права с комплексни филтри")]
        public void TestGetAllPermissionsWithComplexFilters()
        {
            PermissionsRegisterFilters filters = new()
            {
                Name = "Poundnets",
                GroupId = PermissionsData.Npermissions[0].PermissionGroupId,
                TypeIds = new List<int> { PermissionsData.Npermissions[0].PermissionTypeId, PermissionsData.Npermissions[1].PermissionTypeId }
            };

            List<PermissionRegisterDTO> records = service.GetAllPermissions(filters).ToList();

            Assert.NotNull(records);
            Assert.Equal(2, records.Count());
        }

        [Fact(DisplayName = "Извличане на права с филтър свободен текст")]
        public void TestGetAllPermissionsWithFreeTextFilter()
        {
            // name
            PermissionsRegisterFilters filters = new() { FreeTextSearch = "ScientificFishing" };
            List<PermissionRegisterDTO> records = service.GetAllPermissions(filters).ToList();

            Assert.NotNull(records);
            Assert.Equal(2, records.Count);

            // description
            filters = new() { FreeTextSearch = "Излет" };
            records = service.GetAllPermissions(filters).ToList();

            Assert.NotNull(records);
            Assert.Single(records);

            // group
            filters = new() { FreeTextSearch = "риболов" };
            records = service.GetAllPermissions(filters).ToList();

            Assert.NotNull(records);
            Assert.Equal(2, records.Count);

            // type
            filters = new() { FreeTextSearch = "преглед" };
            records = service.GetAllPermissions(filters).ToList();

            Assert.NotNull(records);
            Assert.Single(records);
        }

        [Fact(DisplayName = "Извличане на право")]
        public void TestGetPermission()
        {
            Npermission permission = db.Npermissions
                            .Where(x => x.Id == PermissionsData.Npermissions[0].Id)
                            .Single();
            PermissionRegisterEditDTO result = service.GetPermission(permission.Id);

            AssertMatching(permission, result);
        }

        [Fact(DisplayName = "Редактиране на право")]
        public void TestEditPermission()
        {
            Npermission dbPermission = db.Npermissions
                            .Where(x => x.Id == PermissionsData.Npermissions[1].Id)
                            .Single();

            CopyEntityToDTO(dbPermission, out PermissionRegisterEditDTO permission);
            EditDTO(ref permission);

            service.EditPermission(permission);

            dbPermission = db.Npermissions.Where(x => x.Id == permission.Id).SingleOrDefault();

            Assert.NotNull(dbPermission);
            Assert.Equal(PermissionsData.Npermissions.Count(), db.Npermissions.Count());

            AssertMatching(dbPermission, permission);
        }

        private void SetupDb()
        {
            db.NpermissionTypes.AddRange(PermissionsData.NpermissionTypes);
            db.NpermissionGroups.AddRange(PermissionsData.NpermissionGroups);
            db.Npermissions.AddRange(PermissionsData.Npermissions);
            db.Roles.AddRange(RolesData.Roles);
            db.RolePermissions.AddRange(RolesData.RolePermissions);

            db.SaveChanges();
        }

        private void AssertMatching(Npermission permission, PermissionRegisterEditDTO result)
        {
            Assert.Equal(permission.Id, result.Id);
            Assert.Equal(permission.Name, result.Name);
            Assert.Equal(permission.Description, result.Description);
            Assert.Equal(permission.PermissionGroupId, result.GroupId);
            Assert.Equal(permission.PermissionTypeId, result.TypeId);

            db.Entry(permission).Collection(x => x.RolePermissions).Load();
            Assert.Equal(permission.RolePermissions.Count(x => x.IsActive), result.Roles.Count());
            foreach (RolePermission rolePerm in permission.RolePermissions)
            {
                RolePermissionRegisterDTO role = result.Roles.Where(x => x.RoleId == rolePerm.RoleId).SingleOrDefault();
                if (role == null)
                {
                    Assert.False(rolePerm.IsActive);
                }
                else
                {
                    Assert.Equal(rolePerm.RoleId, role.RoleId);
                    //Assert.Equal(rolePerm.Role.Name, role.RoleId.DisplayName);
                    Assert.Equal(rolePerm.IsActive, role.IsActive);
                }
            }
        }

        private void AssertMatching(Npermission permission, PermissionRegisterDTO result)
        {
            Assert.Equal(permission.Id, result.Id);
            Assert.Equal(permission.Name, result.Name);
            Assert.Equal(permission.Description, result.Description);
            Assert.Equal(permission.PermissionGroup.Name, result.Group);
            Assert.Equal(permission.PermissionType.Name, result.Type);
            Assert.Equal(permission.RolePermissions.Count(x => x.IsActive), result.RolesCount);
        }

        private static void CopyEntityToDTO(Npermission permission, out PermissionRegisterEditDTO result)
        {
            result = new PermissionRegisterEditDTO
            {
                Id = permission.Id,
                Name = permission.Name,
                Description = permission.Description,
                GroupId = permission.PermissionGroupId,
                TypeId = permission.PermissionTypeId,
                Roles = permission.RolePermissions.Select(x => new RolePermissionRegisterDTO
                {
                    PermissionId = x.PermissionId,
                    //RoleId = new NomenclatureDTO
                    //{
                    //    Value = x.RoleId,
                    //    DisplayName = x.Role.Name
                    //},
                    IsActive = x.IsActive
                }).ToList()
            };
        }

        private static void EditDTO(ref PermissionRegisterEditDTO permission)
        {
            permission.Description = $"{permission.Description} editted";
            ++permission.GroupId;

            permission.Roles.Remove(permission.Roles[0]);

            permission.Roles.Add(new RolePermissionRegisterDTO
            {
                //RoleId = new NomenclatureDTO
                //{
                //    Value = RolesData.Roles[1].Id,
                //    DisplayName = RolesData.Roles[1].Name,
                //    IsActive = true
                //},
                IsActive = true
            });

            permission.Roles.Add(new RolePermissionRegisterDTO
            {
                //RoleId = new NomenclatureDTO
                //{
                //    Value = RolesData.Roles[2].Id,
                //    DisplayName = RolesData.Roles[2].Name,
                //    IsActive = true
                //},
                IsActive = true
            });
        }
    }
}
