using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.ConfigModels;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.Common.Resources;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.User;
using IARA.DomainModels.DTOModels.UserManagement;
using IARA.DomainModels.Nomenclatures;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Security;

namespace IARA.Infrastructure.Services
{
    public class MyProfileService : Service, IMyProfileService
    {
        private readonly IPersonService personService;
        private readonly ISecurityEmailSender securityEmailSender;
        private readonly EmailClientSettings emailSettings;

        public MyProfileService(IARADbContext db,
                                IPersonService personService,
                                ISecurityEmailSender securityEmailSender,
                                EmailClientSettings emailSettings)
            : base(db)
        {
            this.personService = personService;
            this.securityEmailSender = securityEmailSender;
            this.emailSettings = emailSettings;
        }

        public MyProfileDTO GetUserProfile(int userId)
        {
            MyProfileDTO userProfile = (from u in this.Db.Users
                                        join p in this.Db.Persons on u.PersonId equals p.Id
                                        join ui in this.Db.UserInfos on u.Id equals ui.UserId
                                        where u.Id == userId
                                        select new MyProfileDTO
                                        {
                                            Id = p.Id,
                                            Username = u.Username,
                                            CitizenshipCountryId = p.CitizenshipCountryId,
                                            HasBulgarianAddressRegistration = p.HasBulgarianAddressRegistration,
                                            FirstName = p.FirstName,
                                            MiddleName = p.MiddleName,
                                            LastName = p.LastName,
                                            EgnLnc = new EgnLncDTO
                                            {
                                                EgnLnc = p.EgnLnc,
                                                IdentifierType = Enum.Parse<IdentifierTypeEnum>(p.IdentifierType)
                                            },
                                            BirthDate = p.BirthDate,
                                            GenderId = p.GenderId,
                                            NewsSubscription = Enum.Parse<NewsSubscriptionTypes>(ui.NewsSubscriptionType)
                                        }).First();

            userProfile.Phone = this.personService.GetPersonPhoneNumber(userProfile.Id);
            userProfile.Document = this.personService.GetPersonDocument(userProfile.Id);
            userProfile.UserAddresses = this.personService.GetAddressRegistrations(userProfile.Id);

            userProfile.Roles = (from u in this.Db.Users
                                 join ur in this.Db.UserRoles on u.Id equals ur.UserId
                                 join r in this.Db.Roles on ur.RoleId equals r.Id
                                 where u.Id == userId
                                 select new RoleDTO
                                 {
                                     Id = ur.RoleId,
                                     Name = r.Name,
                                     AccessValidFrom = ur.AccessValidFrom,
                                     AccessValidTo = ur.AccessValidTo
                                 }).ToList();

            userProfile.Legals = (from u in this.Db.Users
                                  join uLegal in this.Db.UserLegals on u.Id equals uLegal.UserId
                                  join uLegalCurrent in this.Db.Legals on uLegal.LegalId equals uLegalCurrent.Id
                                  where u.Id == userId
                                  select new UserLegalDTO
                                  {
                                      LegalId = uLegal.LegalId,
                                      Name = uLegalCurrent.Name,
                                      EIK = uLegalCurrent.Eik,
                                      Role = uLegal.Role.Name,
                                      Status = Enum.Parse<UserLegalStatusEnum>(uLegal.Status),
                                      IsActive = uLegal.IsActive
                                  }).ToList();

            userProfile.Photo = (from personFile in this.Db.PersonFiles
                                 join photo in this.Db.Files on personFile.FileId equals photo.Id
                                 join fileType in this.Db.NfileTypes on personFile.FileTypeId equals fileType.Id
                                 where personFile.IsActive
                                      && photo.IsActive
                                      && personFile.RecordId == userProfile.Id
                                      && fileType.Code == nameof(FileTypeEnum.PHOTO)
                                 select new FileInfoDTO
                                 {
                                     Id = photo.Id
                                 }).SingleOrDefault();

            if (userProfile.NewsSubscription == NewsSubscriptionTypes.Districts)
            {
                userProfile.NewsDistrictSubscriptions = (from newsSubs in this.Db.NewsDistrictUserSubscriptions
                                                         join district in this.Db.Ndistricts on newsSubs.DistrictId equals district.Id
                                                         where newsSubs.UserId == userId && newsSubs.IsActive
                                                         select new UserNewsDistrictSubscriptionDTO
                                                         {
                                                             Id = newsSubs.DistrictId,
                                                             Name = district.Name
                                                         }).ToList();
            }

            return userProfile;
        }

        public void UpdateUserProfile(MyProfileDTO userProfile, int userId)
        {
            this.AddOrEditUserNewsSubscriptions(userId, userProfile.NewsSubscription, userProfile.NewsDistrictSubscriptions);
            this.Db.AddOrEditPerson(userProfile, userProfile.UserAddresses, userProfile.Id, userProfile.Photo);
            this.Db.SaveChanges();
        }

        public void ChangeUserPassword(int userId, UserPasswordDTO userData)
        {
            User dbUser = (from user in this.Db.Users
                           where user.Id == userId
                           select user).FirstOrDefault();

            if (dbUser != null)
            {
                if (dbUser.Password == CommonUtils.GetPasswordHash(userData.OldPassword, dbUser.Email))
                {
                    dbUser.Password = CommonUtils.GetPasswordHash(userData.NewPassword, dbUser.Email);

                    UserInfo userInfo = (from user in this.Db.UserInfos
                                         where user.UserId == dbUser.Id
                                         select user).First();

                    userInfo.UserMustChangePassword = false;
                    this.Db.SaveChanges();
                }
                else
                {
                    throw new InvalidPasswordException(ErrorResources.msgWrongPassword);
                }
            }
        }

        public bool HasUserEgnLnc(EgnLncDTO egnLnc, int userId)
        {
            bool result = (
                from user in this.Db.Users
                join person in this.Db.Persons on user.PersonId equals person.Id
                where user.Id == userId
                    && person.EgnLnc == egnLnc.EgnLnc
                    && person.IdentifierType == egnLnc.IdentifierType.ToString()
                select user
            ).Any();

            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return this.GetSimpleEntityAuditValues(this.Db.Persons, id);
        }

        private void AddOrEditUserNewsSubscriptions(int userId, NewsSubscriptionTypes? newsSubscription, List<UserNewsDistrictSubscriptionDTO> newNewsDistrictSubscriptions)
        {
            if (newsSubscription != null)
            {
                UserInfo userInfo = this.Db.UserInfos.Where(x => x.UserId == userId).First();
                userInfo.NewsSubscriptionType = newsSubscription.ToString();

                List<NewsDistrictUserSubscription> userNewsSubscriptions = this.Db.NewsDistrictUserSubscriptions.Where(x => x.UserId == userId).ToList();

                foreach (NewsDistrictUserSubscription subscription in userNewsSubscriptions)
                {
                    if (newNewsDistrictSubscriptions != null
                        && newNewsDistrictSubscriptions.Any(x => x.Id == subscription.DistrictId))
                    {
                        subscription.IsActive = true;
                    }
                    else
                    {
                        subscription.IsActive = false;
                    }
                }

                if (newNewsDistrictSubscriptions != null)
                {
                    foreach (UserNewsDistrictSubscriptionDTO newSubscription in newNewsDistrictSubscriptions)
                    {
                        if (!userNewsSubscriptions.Any(x => x.DistrictId == newSubscription.Id))
                        {
                            this.Db.NewsDistrictUserSubscriptions.Add(new NewsDistrictUserSubscription
                            {
                                DistrictId = newSubscription.Id,
                                UserId = userId,
                            });
                        }
                    }
                }
            }
        }
    }
}
