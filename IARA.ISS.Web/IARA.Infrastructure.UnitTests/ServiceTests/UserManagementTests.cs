using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.User;
using IARA.DomainModels.DTOModels.UserManagement;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Fakes.MockupData;
using IARA.Interfaces;
using Newtonsoft.Json;
using Xunit;

namespace IARA.Infrastructure.UnitTests.ServiceTests
{
    public class UserManagementTests
    {
        private IARADbContext db;
        private readonly IUserManagementService service;

        public UserManagementTests(IARADbContext db, IUserManagementService service)
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity("Bearer", "TEST_USER", "TEST_ROLE"));
            this.db = db;
            this.service = service;

            this.db.NterritoryUnits.AddRange(NTerritoryUnitsData.TerritoryUnits);
            this.db.Ndepartments.AddRange(NDepartmentsData.Departments);
            this.db.Nsectors.AddRange(NSectorsData.Sectors);
            this.db.Roles.AddRange(RolesData.Roles);
            this.db.Persons.AddRange(PersonsData.Persons);
            this.db.Users.AddRange(UserManagementData.Users);
            this.db.UserRoles.AddRange(UserManagementData.UserRoles);
            this.db.UserMobileDevices.AddRange(UserManagementData.UserMobileDevices);
            this.db.UserLegals.AddRange(UserManagementData.UserLegals);
            this.db.UserInfos.AddRange(UserManagementData.UserInfos);

            this.db.SaveChanges();
        }

        [Fact(DisplayName = "Извличане на всички вътрешни потребители без филтри")]
        public void TestGetAllInternalUsersEmptyFilters()
        {
            UserManagementFilters filters = new UserManagementFilters();
            IQueryable<UserDTO> result = service.GetAll(filters, true);

            int allInternalUsersCount = db.Users.Where(x => x.IsInternalUser).Count();

            Assert.NotNull(result);
            Assert.Equal(4, allInternalUsersCount);
            Assert.Equal(3, result.Count());
        }

        [Fact(DisplayName = "Извличане на всички външни потребители без филтри")]
        public void TestGetAllExternalUsersEmptyFilters()
        {
            UserManagementFilters filters = new UserManagementFilters();
            IQueryable<UserDTO> result = service.GetAll(filters, false);

            int allExternalUsersCount = db.Users.Where(x => !x.IsInternalUser).Count();

            Assert.NotNull(result);
            Assert.Equal(3, allExternalUsersCount);
            Assert.Equal(3, result.Count());
        }

        [Fact(DisplayName = "Извличане на вътрешни неактивни потребители по зададен текст за търсене")]
        public void TestGetAllInternalUsersFreeTextFilter()
        {
            UserManagementFilters filters = new UserManagementFilters { FreeTextSearch = "petrov", ShowInactiveRecords = true };

            IQueryable<UserDTO> users = service.GetAll(filters, true);

            Assert.NotNull(users);
            Assert.Equal(0, users.Count());
        }

        [Fact(DisplayName = "Извличане на вътрешни активни потребители по зададени комплексни филтри")]
        public void TestGetParametersFilteredInternalUsers()
        {
            UserManagementFilters filters = new UserManagementFilters { FirstName = "Звезда", Email = "z_stambolova@gmail.com" };
            IQueryable<UserDTO> users = service.GetAll(filters, true);

            Assert.NotNull(users);
            Assert.Equal(1, users.Count());
        }

        [Fact(DisplayName = "Извличане на вътрешен потребител по id")]
        public void TestGetInternalUser()
        {
            InternalUserDTO user = service.GetInternalUser(3);
            Assert.NotNull(user);
        }

        [Fact(DisplayName = "Извличане на външен потребител по id")]
        public void TestGetExternalUser()
        {
            ExternalUserDTO user = service.GetExternalUser(6);
            Assert.NotNull(user);
        }

        [Fact(DisplayName = "Извличане на мобилни устройства на потребител по id")]
        public void TestGetUserMobileDevices()
        {
            List<MobileDeviceDTO> mobileDevices = service.GetUserMobileDevices(1);
            Assert.Equal(3, mobileDevices.Count());
        }

        [Fact(DisplayName = "Добавяне на вътрешен потребител")]
        public void TestAddInternalUser()
        {
            InternalUserDTO internalUser = new()
            {
                Username = "ivancho",
                FirstName = "Ivan",
                MiddleName = "Atanasov",
                LastName = "Ivanov",
                Email = "ivancho@gmail.com",
                EGN = "0703310404",
                Phone = "0887623245",
                DepartmentId = NDepartmentsData.Departments[0].Id,
                SectorId = NSectorsData.Sectors[0].Id,
                TerritoryUnitId = TerritoryData.TerritoryUnits[0].Id,
                UserRoles = new List<RoleDTO>()
                {
                    new RoleDTO
                    {
                        Id = RolesData.Roles[0].Id,
                        Name = "Управление на потребители",
                        AccessValidFrom = new DateTime(2021, 4, 1),
                        AccessValidTo = new DateTime (2021, 5, 1)
                    }
                },
                MobileDevices = new List<MobileDeviceDTO>()
                {
                    new MobileDeviceDTO
                    {
                         IMEI = "125ga5r63"
                    }
                }
            };

            service.AddInternalUser(internalUser);
            Assert.Equal(UserManagementData.Users.Where(x => x.IsInternalUser).Count() + 1, db.Users.Where(x => x.IsInternalUser).Count());
        }

        [Fact(DisplayName = "Редактиране на външен потребител")]
        public void TestEditExternalUser()
        {
            ExternalUserDTO externalUser = new()
            {
                Id = UserManagementData.Users[4].Id,
                FirstName = "Ivan",
                MiddleName = "Georgiev",
                LastName = "Georgiev",
                Username = "i_g",
                Email = "i_georgiev@gmail.com",
                EGN = "8506030115",
                Phone = null,
                PersonId = PersonsData.Persons[9].Id,
                UserRoles = new List<RoleDTO>()
                {
                    new RoleDTO
                    {
                        Id = RolesData.Roles[0].Id,
                        Name = "Управление на потребители",
                        AccessValidFrom = new DateTime(2021, 4, 1),
                        AccessValidTo = new DateTime (2021, 4, 1)
                    }
                },
                UserLegals = new List<UserLegalDTO>()
            };

            service.EditExternalUser(externalUser);

            var expected = JsonConvert.SerializeObject(externalUser);
            var actual = JsonConvert.SerializeObject(service.GetExternalUser(5));

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "Редактиране на вътрешен потребител")]
        public void TestEditInternalUser()
        {
            InternalUserDTO internalUser = new()
            {
                Id = UserManagementData.Users[3].Id,
                FirstName = "Звезда",
                MiddleName = "Petrova",
                LastName = "Стамболова",
                Username = "zstambolova",
                Email = "z.stambolova@gmail.comm",
                EGN = "8506030114",
                Phone = null,
                PersonId = PersonsData.Persons[8].Id,
                DepartmentId = NDepartmentsData.Departments[2].Id,
                SectorId = null,
                TerritoryUnitId = NTerritoryUnitsData.TerritoryUnits[1].Id,
                UserRoles = new List<RoleDTO>()
                {
                    new RoleDTO
                    {
                        Id = RolesData.Roles[1].Id,
                        Name = "Администрация",
                        AccessValidFrom = new DateTime(2021, 4, 1),
                        AccessValidTo = new DateTime (2021, 4, 1)
                    }
                },
                MobileDevices = new List<MobileDeviceDTO>()
                {
                    new MobileDeviceDTO
                    {
                        Id = UserManagementData.UserMobileDevices[5].Id,
                        IMEI = "89f8a5rs5982a",
                        Description = "  ",
                        AccessStatus = UserMobileDeviceAccessStatusEnum.Blocked.ToString(),
                        RequestAccessDate = new DateTime(2021, 3, 1)
                    },
                    new MobileDeviceDTO
                    {
                        Id = UserManagementData.UserMobileDevices[6].Id,
                        IMEI = "0f8a5rs5982a",
                        Description = "  ",
                        AccessStatus = UserMobileDeviceAccessStatusEnum.Approved.ToString(),
                        RequestAccessDate = new DateTime(2020, 8, 1)
                    }
                }
            };

            service.EditInternalUser(internalUser);

            var expected = JsonConvert.SerializeObject(internalUser);
            var actual = JsonConvert.SerializeObject(service.GetInternalUser(4));

            Assert.NotNull(actual);
            Assert.Equal(expected, actual);
        }

        [Fact(DisplayName = "Деактивиране на потребител")]
        public void TestDeleteUser()
        {
            User user = db.Users
                .Where(x => x.Id == UserManagementData.Users[0].Id)
                .Single();

            service.Delete(user.Id);
            Assert.True(user.ValidTo < DateTime.Now);
        }

        [Fact(DisplayName = "Активиране на потребител")]
        public void TestUndoDeleteUser()
        {
            User user = db.Users
                .Where(x => x.Id == UserManagementData.Users[1].Id)
                .Single();

            service.UndoDelete(user.Id);
            Assert.True(user.ValidTo > DateTime.Now);
        }
    }
}
