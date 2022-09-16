using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.User;
using IARA.DomainModels.DTOModels.UserManagement;
using IARA.EntityModels.Entities;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.Internal
{
    internal static class InternalUserService
    {
        public static User GetUserByEgnLnch(this IARADbContext db, string egnLnch)
        {
            DateTime now = DateTime.Now;

            User dbUser = (
                from user in db.Users.AsSplitQuery().Include(x => x.UserInfo)
                join person in db.Persons on user.PersonId equals person.Id
                where person.EgnLnc == egnLnch
                    && user.ValidFrom < now
                    && user.ValidTo > now
                select user
            ).SingleOrDefault();

            return dbUser;
        }

        public static EgnLncDTO GetUserEgn(this IARADbContext db, int userId)
        {
            EgnLncDTO egnLnc = (from user in db.Users
                                join person in db.Persons on user.PersonId equals person.Id
                                where user.Id == userId
                                select new EgnLncDTO
                                {
                                    EgnLnc = person.EgnLnc,
                                    IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType)
                                }).Single();
            return egnLnc;
        }

        public static User AddExternalUser(this IARADbContext db, RegixPersonDataDTO regixPersonData, UserRegistrationDTO userRegistration, List<RoleDTO> userRoleDTOs)
        {
            DateTime now = DateTime.Now;

            Person person = db.AddOrEditPerson(regixPersonData);

            string password = null;
            if (userRegistration.Password != null && userRegistration.Password != "")
            {
                password = CommonUtils.GetPasswordHash(userRegistration.Password, userRegistration.Email);
            }

            User entry = new User
            {
                Username = userRegistration.Email,
                Email = userRegistration.Email,
                HasUserPassLogin = userRegistration.HasUserPassLogin.Value,
                HasEauthLogin = userRegistration.HasEAuthLogin.Value,
                ValidFrom = DateTime.Now,
                ValidTo = DefaultConstants.MAX_VALID_DATE,
                IsInternalUser = false,
                Password = password,
                Person = person,
                UserInfo = new UserInfo
                {
                    RegistrationDate = now,
                    IsLocked = false,
                    FailedLoginCount = 0,
                    UserMustChangePassword = false,
                    IsEmailConfirmed = false,
                    NewsSubscriptionType = NewsSubscriptionTypes.ALL.ToString(),
                    HasFishLawConfirmation = true, //TODO?
                    HasTermsAgreementConfirmation = true,
                }
            };

            AddUserRoles(entry, userRoleDTOs);

            db.Users.Add(entry);

            return entry;
        }

        private static void AddUserRoles(User user, List<RoleDTO> userRoles)
        {
            if (userRoles != null)
            {
                foreach (RoleDTO userRole in userRoles)
                {
                    user.UserRoles.Add(new UserRole
                    {
                        AccessValidFrom = userRole.AccessValidFrom,
                        AccessValidTo = userRole.AccessValidTo,
                        IsActive = true,
                        RoleId = userRole.Id
                    });
                }
            }
        }
    }
}
