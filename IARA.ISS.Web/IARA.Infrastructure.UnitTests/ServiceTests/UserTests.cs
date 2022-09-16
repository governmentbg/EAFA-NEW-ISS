using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using IARA.Common.Constants;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.User;
using IARA.EntityModels.Entities;
using IARA.Fakes.MockupData;
using IARA.Interfaces;
using IARA.Security.Enum;
using Xunit;

namespace IARA.Infrastructure.UnitTests.ServiceTests
{
    public class UserTests
    {
        private IARADbContext db;
        private IUserService service;

        public UserTests(IARADbContext dbContext, IUserService service)
        {
            Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity("Bearer", "TEST_USER", "TEST_ROLE"));
            this.db = dbContext;
            this.service = service;

            SetupDb();
        }

        [Fact(DisplayName = "Тества успешно потвърждение за съществуване на активен потребител с определени имейл и парола")]
        public void TestSuccessfulConfirmUserEmailAndPassword()
        {
            string egn = PersonsData.Persons[11].EgnLnc;
            UserLoginDTO user = new UserLoginDTO
            {
                Email = "iivanov@technologica.com",
                Password = "iivanov"
            };

            bool result = this.service.ConfirmUserEmailAndPassword(egn, user);

            Assert.True(result);
        }

        [Fact(DisplayName = "Тества неуспешно потвърждение за съществуване на активен потребител с определени имейл и парола")]
        public void TestFailedConfirmUserEmailAndPassword()
        {
            string egn = PersonsData.Persons[11].EgnLnc;
            UserLoginDTO user = new UserLoginDTO
            {
                Email = "iivanov@technologica.com",
                Password = "iivanov2"
            };

            bool result = this.service.ConfirmUserEmailAndPassword(egn, user);

            Assert.False(result);
        }

        [Fact(DisplayName = "Тества обновяване на информация за потребител при първи логин с еАвт")]
        public void TestUpdateUserEAuthData()
        {
            UserRegistrationDTO user = new UserRegistrationDTO
            {
                CurrentLoginType = LoginTypesEnum.EAUTH,
                EgnLnch = PersonsData.Persons[11].EgnLnc,
                Email = "iivanov@technologica.com",
                FirstName = "Ивелин",
                MiddleName = "Петров",
                LastName = "Караиванов",
                HasEAuthLogin = true,
                HasUserPassLogin = true,
                Password = "iivanov"
            };

            this.service.UpdateUserEAuthData(user);

            DateTime now = DateTime.Now;
            User dbUser = db.Users.Where(x => x.Email == user.Email).Single();
            Person dbPerson = db.Persons.Where(x => x.Id == dbUser.PersonId && x.ValidFrom <= now && x.ValidTo > now).Single();

            AssertUsersDataMatching(user, dbUser);
            AssertPersonDataMatching(user, dbPerson);
        }

        [Fact(DisplayName = "Тества регистриране на външен потребител в системата")]
        public void TestAddExternalUser()
        {
            UserRegistrationDTO user = new UserRegistrationDTO
            {
                //CurrentLoginType = LoginTypesEnum.NEW_REGISTRATION,
                EgnLnch = "9590291313",
                Email = "vivanova@technologica.com",
                FirstName = "Violeta",
                MiddleName = "Ivanova",
                LastName = "Ivanova",
                HasEAuthLogin = false,
                HasUserPassLogin = true,
                Password = "vivanova"
            };
            int PUBLIC_USER_DEFAULT_ROLE_ID = -1;

            int id = this.service.AddExternalUser(user);

            User dbUser = db.Users.Where(x => x.Id == id).Single();
            UserInfo dbUserInfo = db.UserInfos.Where(x => x.UserId == dbUser.Id).Single();
            Person dbPerson = db.Persons.Where(x => x.Id == dbUser.PersonId).Single();
            UserRole dbUserRole = db.UserRoles.Where(x => x.RoleId == PUBLIC_USER_DEFAULT_ROLE_ID && x.UserId == dbUser.Id).SingleOrDefault();

            AssertUserFullDataAsExpected(user, dbUser);
            AssertUserInfosFullDataMatching(dbUserInfo);
            AssertPersonDataMatching(user, dbPerson);
            Assert.NotNull(dbUserRole);
            Assert.Equal(DefaultConstants.MAX_VALID_DATE, dbUserRole.AccessValidTo);
            Assert.True(dbUserRole.IsActive);
        }

        [Fact(DisplayName = "Тества обновяване на информация за потребител след първи логин с еАвт и деактивиран профил с парола")]
        public void TestUpdateUserRegistrationInfo()
        {
            UserRegistrationDTO user = new UserRegistrationDTO
            {
                Email = "ivelin.ivanov@technologica.com",
                EgnLnch = PersonsData.Persons[11].EgnLnc,
                CurrentLoginType = LoginTypesEnum.EAUTH,
                FirstName = "Ивелин",
                MiddleName = "Иванов",
                LastName = "Иванов",
                Password = "iivanov123",
                HasEAuthLogin = true,
                HasUserPassLogin = true
            };

            int id = this.service.UpdateUserRegistrationInfo(user);

            User dbUser = db.Users.Where(x => x.Id == id).Single();
            UserInfo dbUserInfo = db.UserInfos.Where(x => x.UserId == dbUser.Id).Single();
            Person dbPerson = db.Persons.Where(x => x.Id == dbUser.PersonId).Single();

            Assert.Equal(7, id);
            this.AssertUsersDataMatching(user, dbUser);
            Assert.False(dbUserInfo.IsEmailConfirmed);
            this.AssertPersonDataMatching(user, dbPerson);
        }

        [Fact(DisplayName = "Тества деактивиране на профил с парола")]
        public void TestDeactivateUserPasswordAccount()
        {
            string egn = PersonsData.Persons[11].EgnLnc;

            this.service.DeactivateUserPasswordAccount(egn);

            DateTime now = DateTime.Now;
            User user = (from u in db.Users
                         join p in db.Persons on u.PersonId equals p.Id
                         where p.EgnLnc == egn && p.ValidFrom <= now && p.ValidTo > now
                         select u).Single();

            Assert.False(user.HasUserPassLogin);
            Assert.Null(user.Password);
        }

        private void AssertUsersDataMatching(UserRegistrationDTO user, User dbUser)
        {
            Assert.Equal(user.Email, dbUser.Username);
            Assert.Equal(user.Email, dbUser.Email);
            Assert.Equal(user.HasEAuthLogin, dbUser.HasEauthLogin);
            Assert.Equal(user.HasUserPassLogin, dbUser.HasUserPassLogin);
            string password = CommonUtils.GetPasswordHash(user.Password, user.Email);
            Assert.Equal(password, dbUser.Password);
        }

        private void AssertPersonDataMatching(UserRegistrationDTO user, Person dbPerson)
        {
            Assert.Equal(user.FirstName, dbPerson.FirstName);
            Assert.Equal(user.MiddleName, dbPerson.MiddleName);
            Assert.Equal(user.LastName, dbPerson.LastName);
            Assert.Equal(user.EgnLnch, dbPerson.EgnLnc);
        }

        private void AssertUserFullDataAsExpected(UserRegistrationDTO user, User dbUser)
        {
            this.AssertUsersDataMatching(user, dbUser);
            Assert.Equal(DefaultConstants.MAX_VALID_DATE, dbUser.ValidTo);
            Assert.False(dbUser.IsInternalUser);
        }

        private void AssertUserInfosFullDataMatching(UserInfo dbUserInfo)
        {
            Assert.False(dbUserInfo.IsLocked);
            Assert.Equal(0, dbUserInfo.FailedLoginCount);
            Assert.False(dbUserInfo.UserMustChangePassword);
            Assert.False(dbUserInfo.IsEmailConfirmed);
        }

        private void SetupDb()
        {
            db.EmailAddresses.AddRange(PersonsData.EmailAddresses);
            db.Persons.AddRange(PersonsData.Persons);
            db.PersonEmailAddresses.AddRange(PersonsData.PersonEmailAddresses);

            db.Users.AddRange(UserManagementData.Users);
            db.UserInfos.AddRange(UserManagementData.UserInfos);

            db.Roles.AddRange(RolesData.Roles);
            db.UserRoles.AddRange(UserManagementData.UserRoles);

            this.db.SaveChanges();
        }
    }
}
