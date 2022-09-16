using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.Common.Resources;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.User;
using IARA.DomainModels.Nomenclatures;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Logging.Abstractions.Interfaces;
using IARA.Security;
using IARA.Security.Enum;
using IARA.Security.Enums;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public class UserService : Service, IUserService
    {
        private readonly ISecurityEmailSender securityEmailSender;
        private readonly IExtendedLogger logger;
        public UserService(IARADbContext db, ISecurityEmailSender securityEmailSender, IExtendedLogger logger)
            : base(db)
        {
            this.securityEmailSender = securityEmailSender;
            this.logger = logger;
        }

        public int TryUnlockUsers()
        {
            int count = 0;

            try
            {
                List<UserInfo> users = this.Db.UserInfos.Where(x => x.IsLocked
                                                 && x.LockEndDateTime.HasValue
                                                 && x.LockEndDateTime < DateTime.Now)
                                        .ToList();

                foreach (UserInfo user in users)
                {
                    user.IsLocked = false;
                    user.LockEndDateTime = null;
                }

                this.Db.SaveChanges();
            }
            catch (Exception ex)
            {
                this.logger.LogException(ex);
            }

            return count;
        }

        public bool ConfirmUserEmailAndPassword(int userId, UserLoginDTO user)
        {
            DateTime now = DateTime.Now;
            string passwordHash = CommonUtils.GetPasswordHash(user.Password, user.Email);

            User dbUser = (from u in this.Db.Users
                           where u.Id == userId && u.ValidFrom < now && u.ValidTo > now
                           select u).SingleOrDefault();

            if (dbUser != null)
            {
                bool result = dbUser.Password == passwordHash && dbUser.Email == user.Email;

                if (result)
                {
                    dbUser.HasEauthLogin = true;

                    Person dbPerson = (from person in this.Db.Persons
                                       where person.Id == dbUser.PersonId
                                       select person).First();

                    dbPerson.FirstName = user.FirstName;
                    dbPerson.LastName = user.LastName;
                    dbPerson.MiddleName = user.MiddleName;
                    this.Db.SaveChanges();
                }

                return result;
            }
            else
            {
                return false;
            }
        }

        public void UpdateUserEAuthData(UserRegistrationDTO user)
        {
            if (this.UserEmailExists(user.Email))
            {
                throw new EmailExistsException(ErrorResources.msgEmailExists);
            }

            User dbUser = this.Db.GetUserByEgnLnch(user.EgnLnc.EgnLnc);
            dbUser.HasEauthLogin = user.HasEAuthLogin ?? false;
            dbUser.Username = user.Email;
            dbUser.Email = user.Email;
            dbUser.UserInfo.IsEmailConfirmed = false;

            bool hasUserPassLogin = user.HasUserPassLogin.GetValueOrDefault();
            dbUser.HasUserPassLogin = hasUserPassLogin;

            if (hasUserPassLogin)
            {
                string password = CommonUtils.GetPasswordHash(user.Password, user.Email);
                dbUser.Password = password;
            }

            this.UpdatePersonBasicInformation(dbUser, user);
            this.Db.SaveChanges();

            this.SendConfirmationEmail(user.Email);
        }

        public int AddExternalUser(UserRegistrationDTO user)
        {
            if (this.UserEgnLnchExists(user.EgnLnc.EgnLnc))
            {
                throw new EgnLnchExistsException(ErrorResources.msgEgnLnchExists);
            }

            if (this.UserEmailExists(user.Email))
            {
                throw new EmailExistsException(ErrorResources.msgEmailExists);
            }

            DateTime now = DateTime.Now;

            RegixPersonDataDTO regixPersonData = new RegixPersonDataDTO
            {
                EgnLnc = user.EgnLnc,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
                Email = user.Email,
                HasBulgarianAddressRegistration = user.EgnLnc?.IdentifierType == IdentifierTypeEnum.EGN,
            };

            User entry = this.Db.AddExternalUser(regixPersonData, user, null);

            this.Db.SaveChanges();

            this.SendConfirmationEmail(user.Email);

            return entry.Id;
        }

        public int UpdateUserRegistrationInfo(UserRegistrationDTO user)
        {
            User dbUser = this.Db.GetUserByEgnLnch(user.EgnLnc.EgnLnc);
            dbUser.Username = user.Email;
            dbUser.Email = user.Email;
            dbUser.HasEauthLogin = user.HasEAuthLogin ?? false;
            dbUser.HasUserPassLogin = user.HasUserPassLogin ?? false;
            dbUser.UserInfo.IsEmailConfirmed = false;

            if (user.HasUserPassLogin.HasValue && user.HasUserPassLogin.Value == true)
            {
                string password = CommonUtils.GetPasswordHash(user.Password, user.Email);
                dbUser.Password = password;
            }

            this.UpdatePersonBasicInformation(dbUser, user);

            this.Db.SaveChanges();

            return dbUser.Id;
        }

        public void DeactivateUserPasswordAccount(string egnLnch)
        {
            User dbUser = this.Db.GetUserByEgnLnch(egnLnch);
            dbUser.HasUserPassLogin = false;
            dbUser.Password = null;

            this.Db.SaveChanges();
        }

        public string SendConfirmationEmail(string email)
        {
            return this.securityEmailSender.EnqueuePasswordEmail(email, SecurityEmailTypes.NEW_PUBLIC_USER_MAIL);
        }

        public void UpdateUsersActivity(IDictionary<string, DateTime> users)
        {
            throw new NotImplementedException();
        }

        public int GetUserPersonId(int userId)
        {
            int personId = (from user in this.Db.Users
                            where user.Id == userId
                            select user.PersonId).First();

            return personId;
        }

        public int? GetPersonIdByEgnLnc(string egnLnc, IdentifierTypeEnum idType)
        {
            DateTime now = DateTime.Now;

            int personId = (from person in this.Db.Persons
                            where person.EgnLnc == egnLnc
                               && person.IdentifierType == idType.ToString()
                               && person.ValidFrom <= now
                               && person.ValidTo > now
                            select person.Id).FirstOrDefault();

            return personId == 0 ? default(int?) : personId;
        }

        public List<int> GetPersonIdsByUserId(int userId)
        {
            var egnLnc = (from user in this.Db.Users
                          join person in this.Db.Persons on user.PersonId equals person.Id
                          where user.Id == userId
                          select new
                          {
                              person.EgnLnc,
                              person.IdentifierType
                          }).First();

            List<int> personIds = (from person in this.Db.Persons
                                   where person.EgnLnc == egnLnc.EgnLnc
                                        && person.IdentifierType == egnLnc.IdentifierType
                                   select person.Id).ToList();

            return personIds;
        }

        public List<int> GetApprovedLegalIdsByUserId(int userId)
        {
            DateTime now = DateTime.Now;

            List<int> legalIds = (from userLegal in this.Db.UserLegals
                                  where userLegal.UserId == userId
                                    && userLegal.Status == nameof(UserLegalStatusEnum.Approved)
                                    && userLegal.AccessValidFrom <= now
                                    && userLegal.AccessValidTo > now
                                  select userLegal.LegalId).ToList();

            return legalIds;
        }

        public AccountActivationStatusesEnum ActivateUserAccount(UserTokenDTO userTokenInfo)
        {
            DateTime now = DateTime.Now;
            UserInfo dbUserInfo = (from user in this.Db.Users
                                   join userInfo in this.Db.UserInfos on user.Id equals userInfo.UserId
                                   where userInfo.ConfirmEmailKey == userTokenInfo.Token
                                   select userInfo).SingleOrDefault();

            if (dbUserInfo != null)
            {
                if (dbUserInfo.EmailKeyValidTo > now)
                {
                    dbUserInfo.IsEmailConfirmed = true;
                    dbUserInfo.ConfirmEmailKey = null;
                    this.Db.SaveChanges();

                    return AccountActivationStatusesEnum.Successful;
                }
                else
                {
                    return AccountActivationStatusesEnum.TokenExpired;
                }
            }
            else
            {
                return AccountActivationStatusesEnum.TokenNonExistent;
            }
        }

        public string GetUserEmailByConfirmationKey(string token)
        {
            string email = (from user in this.Db.Users
                            join userInfo in this.Db.UserInfos on user.Id equals userInfo.UserId
                            where userInfo.ConfirmEmailKey == token
                            select user.Email).Single();

            return email;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.Users, id);
        }

        public UserAuthDTO GetUserAuthInfo(int userId)
        {
            return (from x in this.Db.Users
                    join y in this.Db.Persons on x.PersonId equals y.Id
                    join z in this.Db.UserInfos on x.Id equals z.UserId
                    where x.Id == userId
                    select new UserAuthDTO
                    {
                        ID = x.Id,
                        EgnLnc = new EgnLncDTO
                        {
                            EgnLnc = y.EgnLnc,
                            IdentifierType = Enum.Parse<IdentifierTypeEnum>(y.IdentifierType)
                        },
                        Username = x.Username,
                        FirstName = y.FirstName,
                        LastName = y.LastName,
                        MiddleName = y.MiddleName,
                        HasEAuthLogin = x.HasEauthLogin,
                        HasUserPassLogin = x.HasUserPassLogin,
                        IsInternalUser = x.IsInternalUser,
                        CurrentLoginType = LoginTypesEnum.PASSWORD,
                        UserMustChangePassword = z.UserMustChangePassword,
                        UserMustChangeData = z.UserMustResetProfileData,
                        PersonId = y.Id
                    }).First();
        }

        public UserAuthDTO GetUserAuthInfo(string identifier)
        {
            if (EAuthUtils.TryParseIdentifier(identifier, out IdentificationModel identification))
            {


                Person person = (from p in this.Db.Persons
                                 where p.EgnLnc == identification.Identifier &&
                                       p.ValidTo == DefaultConstants.MAX_VALID_DATE
                                 select p).FirstOrDefault();

                if (person != null)
                {
                    int userId = (from p in this.Db.Persons
                                  join user in this.Db.Users on p.Id equals user.PersonId
                                  where p.Id == person.Id
                                  select user.Id).FirstOrDefault();

                    if (userId != 0)
                    {
                        UserAuthDTO user = this.GetUserAuthInfo(userId);
                        user.CurrentLoginType = LoginTypesEnum.EAUTH;

                        return user;
                    }
                    else
                    {
                        return new UserAuthDTO
                        {
                            ID = 0,
                            EgnLnc = new EgnLncDTO
                            {
                                EgnLnc = identifier,
                                IdentifierType = this.ConvertIdentifierType(identification.IdentifierType)
                            },
                            CurrentLoginType = LoginTypesEnum.EAUTH,
                            HasEAuthLogin = false,
                            HasUserPassLogin = false,
                            IsInternalUser = false,
                            FirstName = person.FirstName,
                            MiddleName = person.MiddleName,
                            LastName = person.LastName
                        };
                    }
                }
                else
                {
                    return new UserAuthDTO
                    {
                        EgnLnc = new EgnLncDTO
                        {
                            EgnLnc = identifier,
                            IdentifierType = this.ConvertIdentifierType(identification.IdentifierType)
                        },
                        CurrentLoginType = LoginTypesEnum.EAUTH,
                        HasEAuthLogin = false,
                        HasUserPassLogin = false,
                        IsInternalUser = false,
                    };
                }
            }
            else
            {
                return null;
            }
        }

        public bool ConfirmEmailToken(string token)
        {
            UserInfo userInfo = this.Db.UserInfos.Where(x => x.ConfirmEmailKey == token).SingleOrDefault();

            if (userInfo != null && userInfo.EmailKeyValidTo.HasValue && userInfo.EmailKeyValidTo < DateTime.Now)
            {
                this.ResetUserInfo(userInfo);

                this.Db.SaveChanges();

                return true;
            }

            return false;
        }

        public string GetUserPhoto(int id)
        {
            DateTime now = DateTime.Now;

            string result = (from u in this.Db.Users
                             join pFile in this.Db.PersonFiles on u.PersonId equals pFile.RecordId
                             join photo in this.Db.Files on pFile.FileId equals photo.Id
                             join fType in this.Db.NfileTypes on pFile.FileTypeId equals fType.Id
                             where u.Id == id
                                && pFile.IsActive
                                && photo.IsActive
                                && fType.Code == FileTypeEnum.PHOTO.ToString()
                             select $"data:{photo.MimeType};base64,{Convert.ToBase64String(photo.Content)}").SingleOrDefault();

            return result;
        }

        public void ChangeUserPassword(UserChangePasswordDTO data)
        {
            DateTime now = DateTime.Now;
            User user = (from u in this.Db.Users
                         join uInfo in this.Db.UserInfos on u.Id equals uInfo.UserId
                         where uInfo.ConfirmEmailKey == data.Token && u.ValidFrom <= now && u.ValidTo > now
                         select u).SingleOrDefault();

            if (user != null)
            {
                user.Password = CommonUtils.GetPasswordHash(data.Password, user.Email);
                UserInfo userInfo = this.Db.UserInfos.Where(x => x.UserId == user.Id).Single();

                this.ResetUserInfo(userInfo);
                this.Db.SaveChanges();
            }
            else
            {
                throw new ArgumentException(string.Format(ErrorResources.passwordChangeErr, data.Token));
            }
        }

        public List<int> GetUserIdsInSameTerritoryUnitAsUser(int userId)
        {
            int? territoryUnitId = (from user in this.Db.Users
                                    join info in this.Db.UserInfos on user.Id equals info.UserId
                                    where user.Id == userId
                                    select info.TerritoryUnitId).First();

            if (!territoryUnitId.HasValue)
            {
                return null;
            }

            List<int> ids = (from user in this.Db.Users
                             join info in this.Db.UserInfos on user.Id equals info.UserId
                             where info.TerritoryUnitId == territoryUnitId.Value
                             select user.Id).ToList();
            return ids;
        }

        public int? GetUserTerritoryUnitId(int userId)
        {
            return (from userInfo in this.Db.UserInfos
                    where userInfo.UserId == userId
                    select userInfo.TerritoryUnitId).First();
        }

        public ChangeUserDataDTO GetAllUserData(int userID)
        {
            ChangeUserDataDTO userData = (from user in this.Db.Users
                                          join userInfo in this.Db.UserInfos on user.Id equals userInfo.UserId
                                          join person in this.Db.Persons on user.PersonId equals person.Id
                                          join gender in this.Db.Ngenders on person.GenderId equals gender.Id into left
                                          from gender in left.DefaultIfEmpty()
                                          join country in this.Db.Ncountries on person.CitizenshipCountryId equals country.Id into left1
                                          from country in left1.DefaultIfEmpty()
                                          where user.Id == userID
                                          select new ChangeUserDataDTO
                                          {
                                              Id = userID,
                                              Email = user.Email,
                                              FirstName = person.FirstName,
                                              Username = user.Username,
                                              MiddleName = person.MiddleName,
                                              LastName = person.LastName,
                                              HasBulgarianAddressRegistration = person.HasBulgarianAddressRegistration,
                                              BirthDate = person.BirthDate,
                                              EgnLnc = new EgnLncDTO
                                              {
                                                  EgnLnc = person.EgnLnc,
                                                  IdentifierType = Enum.Parse<IdentifierTypeEnum>(person.IdentifierType),
                                              },
                                              GenderName = gender != null ? gender.Name : default,
                                              GenderId = gender != null ? gender.Id : default(int?),
                                              CitizenshipCountryId = person.CitizenshipCountryId,
                                              CitizenshipCountryName = country != null ? country.Name : default
                                          }).FirstOrDefault();

            return userData;
        }

        public bool UpdateUserInfo(ChangeUserDataDTO userData)
        {
            User user = this.Db.Users.Include(x => x.UserInfo).Where(x => x.Id == userData.Id).FirstOrDefault();

            if (user != null && user.UserInfo.UserMustResetProfileData)
            {
                if (this.Db.Users.Where(x => x.Email == userData.Email).Any())
                {
                    throw new ValidationException(ErrorResources.msgExistingEmail);
                }

                user.Username = userData.Username;
                user.Email = userData.Username;

                this.Db.AddOrEditPerson(userData, userData.UserAddresses, user.PersonId);
                user.UserInfo.UserMustResetProfileData = false;

                if (!string.IsNullOrEmpty(userData.Password))
                {
                    user.Password = CommonUtils.GetPasswordHash(userData.Password, user.Email);
                    user.UserInfo.UserMustChangePassword = false;
                }

                this.Db.SaveChanges();
                this.SendConfirmationEmail(user.Email);
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UpdatePersonBasicInformation(User dbUser, UserRegistrationDTO user)
        {
            this.Db.Entry(dbUser).Reference(x => x.Person).Load();
            dbUser.Person.EgnLnc = user.EgnLnc.EgnLnc;
            dbUser.Person.IdentifierType = user.EgnLnc.IdentifierType.ToString();
            dbUser.Person.FirstName = user.FirstName;
            dbUser.Person.MiddleName = user.MiddleName;
            dbUser.Person.LastName = user.LastName;
        }

        private bool UserEmailExists(string email)
        {
            bool result = (from user in this.Db.Users
                           where user.Email == email
                           select user.Id).Any();
            return result;
        }

        private bool UserEgnLnchExists(string egnLnch)
        {
            bool result = this.Db.GetUserByEgnLnch(egnLnch) != null;
            return result;
        }

        private IdentifierTypeEnum ConvertIdentifierType(UserIdentifierTypes identifierType)
        {
            switch (identifierType)
            {
                case UserIdentifierTypes.UNKNOWN:
                case UserIdentifierTypes.EGN:
                    return IdentifierTypeEnum.EGN;
                case UserIdentifierTypes.LNCH:
                    return IdentifierTypeEnum.LNC;
                default:
                    throw new NotImplementedException("Непозната стойнонст на идентификатор");
            }
        }

        private void ResetUserInfo(UserInfo userInfo)
        {
            userInfo.ConfirmEmailKey = null;
            userInfo.IsEmailConfirmed = true;
            userInfo.UserMustChangePassword = false;
            userInfo.IsLocked = false;
            userInfo.FailedLoginCount = 0;
        }
    }
}
