using System;
using System.Collections.Generic;
using System.Linq;
using IARA.DataAccess;
using IARA.EntityModels.Entities;
using IARA.Logging.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TL.Dependency.Abstractions;

namespace IARA.Tests
{
    public class UsersMigration
    {
        private IServiceProvider serviceProvider;
        public UsersMigration(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }


        private const string DBConnectionMigration = "Host=172.23.163.78;Port=5432;Database=iss-migration;Username=iara;Password=zFy5Ugps;SslMode=Prefer;Trust Server Certificate=true;Include Error Detail=true;";
        private const string DBConnection = "Host=172.23.163.78;Port=5432;Database=iss;Username=iara;Password=zFy5Ugps;SslMode=Prefer;Trust Server Certificate=true;Include Error Detail=true;";

        public void MigrateUsers()
        {
            using IARADbContext fromDbContext = BuildDbContext(serviceProvider, DBConnectionMigration);
            using IARADbContext toDbContext = BuildDbContext(serviceProvider, DBConnection);
            User toUser, fromUser;
            UserInfo toUserInfo, fromUserInfo;
            Person toPerson, fromPerson;

            var transaction = toDbContext.BeginTransaction();

            try
            {
                foreach (var foreachUser in fromDbContext.Users.Where(x => !(new int[] { 0, -1, -2 }).Contains(x.Id)).ToList())
                {
                    fromUser = foreachUser;

                    toUser = toDbContext.Users.Where(x => x.Username == fromUser.Username).FirstOrDefault();

                    if (toUser != null)
                    {
                        toUserInfo = toDbContext.UserInfos.Where(x => x.UserId == toUser.Id).FirstOrDefault();

                        if (toUserInfo == null)
                        {
                            toUserInfo = new UserInfo();
                            toUser.UserInfo = toUserInfo;
                            toDbContext.UserInfos.Add(toUserInfo);
                        }

                        toPerson = toDbContext.Persons.Where(x => x.Id == toUser.PersonId).First();

                        fromUserInfo = fromDbContext.UserInfos.Where(x => x.UserId == fromUser.Id).FirstOrDefault();

                        if (fromUserInfo == null)
                        {
                            fromUserInfo = new UserInfo()
                            {
                                FailedLoginCount = 0,
                                IsEmailConfirmed = true,
                                IsLocked = true,
                                UserMustChangePassword = true,
                                UserMustResetProfileData = true,
                                HasFishLawConfirmation = true,
                                HasTermsAgreementConfirmation = true,
                                //TerritoryUnitId = 0,
                                //SectorId = 0,
                                //DepartmentId = 0,
                                RegistrationDate = toUser.CreatedOn,
                                ConfirmEmailKey = Guid.NewGuid().ToString().Replace("-", ""),
                                NewsSubscriptionType = "None",
                                User = fromUser,
                                UserId = fromUser.Id
                            };
                        }

                        fromPerson = fromDbContext.Persons.Where(x => x.Id == fromUser.PersonId).First();

                        UpdateUser(toUser, fromUser);
                        UpdateUserInfo(toUserInfo, fromUserInfo);
                        UpdatePerson(toPerson, fromPerson);
                        UpdateRoles(fromUser, toUser, fromDbContext, toDbContext);
                    }
                    else
                    {
                        fromUserInfo = fromDbContext.UserInfos.Where(x => x.UserId == fromUser.Id).First();
                        fromPerson = fromDbContext.Persons.Where(x => x.Id == fromUser.PersonId).First();

                        toUser = new User();
                        toUserInfo = new UserInfo();
                        toPerson = new Person();

                        toUser.Person = toPerson;
                        toUser.UserInfo = toUserInfo;

                        UpdateUser(toUser, fromUser);
                        UpdateUserInfo(toUserInfo, fromUserInfo);
                        UpdatePerson(toPerson, fromPerson);
                        UpdateRoles(fromUser, toUser, fromDbContext, toDbContext);
                    }

                    toDbContext.SaveChanges();
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                transaction.Rollback();
            }
        }

        private void UpdateRoles(User fromUser, User toUser, IARADbContext fromDbContext, IARADbContext toDbContext)
        {
            List<UserRole> toUserRoles;

            if (toUser.Id != 0 || toUser.Username == "Administrator@IARA.eGov.BG")
            {
                toUserRoles = toDbContext.UserRoles.Include(x => x.Role).Where(x => x.UserId == toUser.Id).ToList();
            }
            else
            {
                toUserRoles = new List<UserRole>();
            }

            foreach (var fromUserRole in fromDbContext.UserRoles.Include(x => x.Role).Where(x => x.UserId == fromUser.Id).ToList())
            {
                var role = toDbContext.Roles.Where(x => x.Code == fromUserRole.Role.Code).FirstOrDefault();
                var toUserRole = toUserRoles.Where(x => x?.Role.Code == role.Code).FirstOrDefault();

                if (toUserRole != null)
                {
                    toUserRole.AccessValidFrom = fromUserRole.AccessValidFrom;
                    toUserRole.AccessValidTo = fromUserRole.AccessValidTo;
                    toUserRole.IsActive = fromUserRole.IsActive;
                }
                else
                {
                    toDbContext.UserRoles.Add(new UserRole
                    {
                        Role = role,
                        User = toUser,
                        AccessValidFrom = fromUserRole.AccessValidFrom,
                        AccessValidTo = fromUserRole.AccessValidTo,
                        IsActive = fromUserRole.IsActive
                    });
                }
            }
        }

        private void UpdateUser(User toUser, User fromUser)
        {
            SetUsername(toUser, fromUser);
            toUser.HasEauthLogin = fromUser.HasEauthLogin;
            toUser.HasUserPassLogin = fromUser.HasUserPassLogin;
            toUser.IsInternalUser = fromUser.IsInternalUser;
            toUser.Password = fromUser.Password;
            toUser.PersonId = fromUser.PersonId;
            toUser.ValidFrom = fromUser.ValidFrom;
            toUser.ValidTo = fromUser.ValidTo;
        }

        private void SetUsername(User toUser, User fromUser)
        {
            if (!string.IsNullOrEmpty(fromUser.Email))
            {
                toUser.Email = fromUser.Email;
                toUser.Username = fromUser.Email;
            }
            else if (!string.IsNullOrEmpty(fromUser.Username))
            {
                toUser.Email = fromUser.Username;
                toUser.Username = fromUser.Username;
            }
            else if (!string.IsNullOrEmpty(toUser.Username))
            {
                toUser.Email = toUser.Email;
            }
            else if (!string.IsNullOrEmpty(toUser.Email))
            {
                toUser.Username = toUser.Email;
            }
            else
            {
                throw new ArgumentNullException(nameof(toUser.Username));
            }
        }

        private void UpdateUserInfo(UserInfo toUser, UserInfo fromUser)
        {
            toUser.TerritoryUnitId = fromUser.TerritoryUnitId;
            toUser.Position = fromUser.Position;
            toUser.ConfirmEmailKey = fromUser.ConfirmEmailKey;
            toUser.DepartmentId = fromUser.DepartmentId;
            toUser.EmailKeyValidTo = fromUser.EmailKeyValidTo;
            toUser.FailedLoginCount = fromUser.FailedLoginCount;
            toUser.HasFishLawConfirmation = fromUser.HasFishLawConfirmation;
            toUser.HasTermsAgreementConfirmation = fromUser.HasTermsAgreementConfirmation;
            toUser.IsEmailConfirmed = fromUser.IsEmailConfirmed;
            toUser.IsLocked = fromUser.IsLocked;
            toUser.LastFailedLoginAttempt = fromUser.LastFailedLoginAttempt;
            toUser.LastLoginDate = fromUser.LastLoginDate;
            toUser.UserMustResetProfileData = fromUser.UserMustResetProfileData;
            toUser.UserMustChangePassword = fromUser.UserMustChangePassword;
            toUser.RegistrationDate = fromUser.RegistrationDate;
            toUser.SectorId = fromUser.SectorId;
            toUser.NewsSubscriptionType = fromUser.NewsSubscriptionType;
            toUser.LockEndDateTime = fromUser.LockEndDateTime;
        }

        private void UpdatePerson(Person toPerson, Person fromPerson)
        {
            toPerson.BirthDate = fromPerson.BirthDate;
            toPerson.CitizenshipCountryId = fromPerson.CitizenshipCountryId;
            toPerson.Comments = fromPerson.Comments;
            toPerson.EgnLnc = fromPerson.EgnLnc;
            toPerson.FirstName = GetCorrectValue(fromPerson.FirstName);
            toPerson.GenderId = fromPerson.GenderId;
            toPerson.HasBulgarianAddressRegistration = fromPerson.HasBulgarianAddressRegistration;
            toPerson.IdentifierType = fromPerson.IdentifierType;
            toPerson.LastName = GetCorrectValue(fromPerson.LastName);
            toPerson.MiddleName = GetCorrectValue(fromPerson.MiddleName);
            toPerson.ValidFrom = fromPerson.ValidFrom;
            toPerson.ValidTo = fromPerson.ValidTo;
        }

        private string GetCorrectValue(string value)
        {
            if (value != null)
            {
                value = value.Trim();

                if (string.IsNullOrEmpty(value))
                {
                    return "missing";
                }
                else
                {
                    return value;
                }
            }

            return value;
        }

        private IARADbContext BuildDbContext(IServiceProvider serviceProvider, string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder();

            optionsBuilder = optionsBuilder.UseNpgsql(connectionString, x =>
            {
                x.UseNetTopologySuite();
            });

            var options = optionsBuilder.Options;

            var contextOptions = new DbContextOptions<BaseIARADbContext>(options.Extensions.ToDictionary(x => x.GetType(), x => x));

            var baseContext = new BaseIARADbContext(contextOptions);

            IUserActionsAuditLogger userActionsAuditLogger = serviceProvider.GetService<IUserActionsAuditLogger>();
            var providerFactory = serviceProvider.GetService<IScopedServiceProviderFactory>();

            return new IARADbContext(baseContext, userActionsAuditLogger, providerFactory);
        }

    }
}
