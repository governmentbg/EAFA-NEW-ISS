using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.RolesRegister;
using IARA.DomainModels.DTOModels.UserManagement;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Fakes.MockupData;
using IARA.Interfaces;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IARA.Infrastructure.UnitTests.ServiceTests
{
    public class RolesRegisterTests
    {
        private readonly IARADbContext db;
        private readonly IRolesRegisterService service;

        public RolesRegisterTests(IARADbContext db, IRolesRegisterService service)
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity("Bearer", "TEST_USER", "TEST_ROLE"));
            this.db = db;
            this.service = service;

            SetupDb();
        }

        [Fact(DisplayName = "Извличане на всички активни роли")]
        public void TestGetAllActiveRoles()
        {
            RolesRegisterFilters filters = new() { ShowInactiveRecords = false };
            List<RoleRegisterDTO> records = service.GetAllRoles(filters).ToList();

            List<Role> dbRecords = GetActiveRoles(DateTime.Now);
            Assert.Equal(records.Count, dbRecords.Count);

            foreach (Role dbRole in dbRecords)
            {
                RoleRegisterDTO role = records.Where(x => x.Id == dbRole.Id).SingleOrDefault();
                Assert.NotNull(role);
                AssertMatching(role, dbRole);
            }
        }

        [Fact(DisplayName = "Извличане на всички неактивни роли")]
        public void TestGetAllInctiveRoles()
        {
            RolesRegisterFilters filters = new() { ShowInactiveRecords = true };
            List<RoleRegisterDTO> records = service.GetAllRoles(filters).ToList();

            List<Role> dbRecords = GetInactiveRoles(DateTime.Now);
            Assert.Equal(records.Count, dbRecords.Count);

            foreach (Role dbRole in dbRecords)
            {
                RoleRegisterDTO role = records.Where(x => x.Id == dbRole.Id).SingleOrDefault();
                Assert.NotNull(role);
                AssertMatching(role, dbRole);
            }
        }

        [Fact(DisplayName = "Извличане на роли с комплексни филтри")]
        public void TestGetAllRolesWithComplexFilters()
        {
            RolesRegisterFilters filters = new()
            {
                Code = "public",
                Name = "потребител",
                PermissionId = PermissionsData.Npermissions[0].Id,
                ValidFrom = new DateTime(2020, 4, 26)
            };
            List<RoleRegisterDTO> records = service.GetAllRoles(filters).ToList();

            Assert.Single(records);

            Role dbRole = db.Roles.Where(x => x.Id == RolesData.Roles[3].Id).SingleOrDefault();
            Assert.NotNull(dbRole);

            RoleRegisterDTO role = records[0];
            AssertMatching(role, dbRole);
        }

        [Fact(DisplayName = "Извличане на роли с филтър свободен текст - код")]
        public void TestGetAllRolesWithFreeTextFilterCode()
        {
            string text = "Up";
            List<Role> dbRecords = GetActiveRoles(DateTime.Now).Where(x => x.Code.ToLower().Contains(text)).ToList();
            TestFreeTextSearch(text, dbRecords);
        }

        [Fact(DisplayName = "Извличане на роли с филтър свободен текст - име")]
        public void TestGetAllRolesWithFreeTextFilterName()
        {
            string text = "админ";
            List<Role> dbRecords = GetActiveRoles(DateTime.Now).Where(x => x.Name.ToLower().Contains(text)).ToList();
            TestFreeTextSearch(text, dbRecords);
        }

        [Fact(DisplayName = "Извличане на роли с филтър свободен текст - описание")]
        public void TestGetAllRolesWithFreeTextFilterDescription()
        {
            string text = "страници";
            List<Role> dbRecords = GetActiveRoles(DateTime.Now).Where(x => x.Description?.ToLower().Contains(text) ?? false).ToList();
            TestFreeTextSearch(text, dbRecords);
        }

        [Fact(DisplayName = "Извличане на роли с филтър свободен текст - дата")]
        public void TestGetAllRolesWithFreeTextFilterDate()
        {
            string text = "4.26.2021";
            DateTime? date = DateTimeUtils.TryParseDate(text);

            List<Role> dbRecords = GetActiveRoles(DateTime.Now).Where(x => x.ValidFrom == date || x.ValidTo == date).ToList();
            TestFreeTextSearch(text, dbRecords);
        }


        [Fact(DisplayName = "Извличане на потребители с роля")]
        public void TestGetUsersWithRole()
        {
            DateTime now = DateTime.Now;

            List<UserRole> dbUserRoles = db.UserRoles.Where(x => x.RoleId == RolesData.Roles[0].Id
                                                        && x.IsActive
                                                        && x.AccessValidFrom <= now
                                                        && x.AccessValidTo >= now
                                                        && x.User.ValidFrom <= now
                                                        && x.User.ValidTo >= now).ToList();

            List<NomenclatureDTO> users = service.GetUsersWithRole(RolesData.Roles[0].Id);

            Assert.Equal(dbUserRoles.Count, users.Count);
            foreach (NomenclatureDTO user in users)
            {
                UserRole dbUserRole = dbUserRoles.Where(x => x.UserId == user.Value).SingleOrDefault();
                Assert.NotNull(dbUserRole);

                User dbUser = dbUserRole.User;
                Assert.NotNull(dbUser);
                Assert.Equal(dbUser.Id, user.Value);
                Assert.Equal($"{dbUser.Person.FirstName} {dbUser.Person.LastName} ({dbUser.Username})", user.DisplayName);
            }
        }


        [Fact(DisplayName = "Изтриване на роля")]
        public void TestDeleteRole()
        {
            Role role = db.Roles.Where(x => x.Id == RolesData.Roles[4].Id).Single();
            service.DeleteRole(role.Id);

            DateTime now = DateTime.Now;
            Assert.True(role.ValidFrom >= now || role.ValidTo < now);
        }

        [Fact(DisplayName = "Изтриване и замяна на роля")]
        public void TestDeleteAndReplaceRole()
        {
            DateTime now = DateTime.Now.AddSeconds(15);

            Role oldRole = db.Roles.Include(x => x.UserRoles).Where(x => x.Id == RolesData.Roles[0].Id).SingleOrDefault();
            Role newRole = db.Roles.Include(x => x.UserRoles).Where(x => x.Id == RolesData.Roles[1].Id).SingleOrDefault();
            int oldRoleActiveUsersCount = oldRole.UserRoles.Count(x => x.AccessValidFrom <= now && x.AccessValidTo >= now);
            int newRoleActiveUsersCount = newRole.UserRoles.Count(x => x.AccessValidFrom <= now && x.AccessValidTo >= now);

            service.DeleteAndReplaceRole(RolesData.Roles[0].Id, RolesData.Roles[1].Id);

            oldRole = db.Roles.Include(x => x.UserRoles).Where(x => x.Id == RolesData.Roles[0].Id).SingleOrDefault();
            newRole = db.Roles.Include(x => x.UserRoles).Where(x => x.Id == RolesData.Roles[1].Id).SingleOrDefault();

            Assert.NotNull(oldRole);
            Assert.NotNull(newRole);
            Assert.False(oldRole.ValidFrom <= now && oldRole.ValidTo >= now);

            List<UserRole> oldRoleActiveUsers = oldRole.UserRoles.Where(x => x.AccessValidFrom <= now && x.AccessValidTo >= now).ToList();
            List<UserRole> newRoleActiveUsers = newRole.UserRoles.Where(x => x.AccessValidFrom <= now && x.AccessValidTo >= now).ToList();

            Assert.Empty(oldRoleActiveUsers);
            Assert.Equal(newRoleActiveUsers.Count, oldRoleActiveUsersCount + newRoleActiveUsersCount);
        }

        [Fact(DisplayName = "Възстановяване на роля")]
        public void TestRestoreRole()
        {
            Role role = db.Roles.Where(x => x.Id == RolesData.Roles[0].Id).Single();
            service.UndoDeleteRole(role.Id);

            DateTime now = DateTime.Now;
            Assert.True(role.ValidFrom <= now && role.ValidTo > now);
        }

        private List<Role> GetActiveRoles(DateTime now)
        {
            return db.Roles.Where(x => x.ValidFrom <= now && x.ValidTo >= now).ToList();
        }

        private List<Role> GetInactiveRoles(DateTime now)
        {
            return db.Roles.Where(x => x.ValidFrom > now || x.ValidTo < now).ToList();
        }

        private void TestFreeTextSearch(string text, List<Role> dbRecords)
        {
            RolesRegisterFilters filters = new() { FreeTextSearch = text };
            List<RoleRegisterDTO> records = service.GetAllRoles(filters).ToList();

            Assert.Equal(records.Count, dbRecords.Count);

            foreach (Role dbRole in dbRecords)
            {
                RoleRegisterDTO role = records.Where(x => x.Id == dbRole.Id).SingleOrDefault();
                Assert.NotNull(role);
                AssertMatching(role, dbRole);
            }
        }

        private void SetupDb()
        {
            db.NpermissionGroups.AddRange(PermissionsData.NpermissionGroups);
            db.NpermissionTypes.AddRange(PermissionsData.NpermissionTypes);
            db.Npermissions.AddRange(PermissionsData.Npermissions);
            db.Roles.AddRange(RolesData.Roles);
            db.RolePermissions.AddRange(RolesData.RolePermissions);

            db.Ncountries.AddRange(AddressData.Countries);
            db.Persons.AddRange(PersonsData.Persons);
            db.Users.AddRange(UserManagementData.Users);
            db.UserRoles.AddRange(UserManagementData.UserRoles);

            db.SaveChanges();
        }

        private void AssertMatching(RoleRegisterDTO role, Role dbRole)
        {
            Assert.Equal(dbRole.Id, role.Id);
            Assert.Equal(dbRole.Code, role.Code);
            Assert.Equal(dbRole.Name, role.Name);
            Assert.Equal(dbRole.Description, role.Description);
            Assert.Equal(dbRole.ValidFrom, role.ValidFrom);
            Assert.Equal(dbRole.ValidTo, role.ValidTo);

            DateTime now = DateTime.Now;

            db.Entry(dbRole).Collection(x => x.UserRoles).Load();
            Assert.Equal(dbRole.UserRoles.Count(x => x.IsActive
                                                && x.AccessValidFrom <= now
                                                && x.AccessValidTo >= now
                                                && x.User.ValidFrom <= now
                                                && x.User.ValidTo >= now), role.UsersCount);

            if (role.IsActive)
            {
                Assert.True(dbRole.ValidFrom <= now && dbRole.ValidTo >= now);
            }
            else
            {
                Assert.False(dbRole.ValidFrom <= now && dbRole.ValidTo >= now);
            }
        }


        private static void EditDTO(RoleRegisterEditDTO role)
        {
            role.Name = $"{role.Name} editted";
            role.Description = $"{role.Description} editted";
            role.ValidFrom = role.ValidFrom.Value.AddDays(-12);
            role.ValidTo = role.ValidTo.Value.AddDays(12);
            role.HasInternalAccess = !role.HasInternalAccess;
            role.HasPublicAccess = !role.HasPublicAccess;

            role.PermissionIds.Remove(PermissionsData.Npermissions[0].Id);
            role.PermissionIds.Add(PermissionsData.Npermissions[5].Id);
        }
    }
}
