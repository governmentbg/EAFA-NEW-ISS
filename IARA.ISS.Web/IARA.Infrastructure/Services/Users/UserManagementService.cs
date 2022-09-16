using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.ConfigModels;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.User;
using IARA.DomainModels.DTOModels.UserManagement;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Enums;

namespace IARA.Infrastructure.Services
{
    public class UserManagementService : Service, IUserManagementService
    {
        private readonly IPersonService personService;
        private readonly EmailClientSettings emailSettings;
        private readonly ISecurityEmailSender securityEmailSender;

        public UserManagementService(IARADbContext db,
            IPersonService personService,
            ISecurityEmailSender securityEmailSender,
            EmailClientSettings emailSettings)
                 : base(db)
        {
            this.personService = personService;
            this.emailSettings = emailSettings;
            this.securityEmailSender = securityEmailSender;
        }

        public void AddInternalUser(InternalUserDTO user)
        {
            DateTime now = DateTime.Now;

            RegixPersonDataDTO regixPersonData = new RegixPersonDataDTO()
            {
                Email = user.Email,
                EgnLnc = user.EgnLnc,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Phone = user.Phone
            };

            Person person = this.Db.AddOrEditPerson(regixPersonData);

            User dbUser = new User()
            {
                Email = user.Email,
                Username = user.Username,
                Person = person,
                IsInternalUser = true,
                ValidFrom = now,
                ValidTo = DateTime.MaxValue,
                HasUserPassLogin = true,
                HasEauthLogin = false
            };

            dbUser.UserInfo = new UserInfo()
            {
                RegistrationDate = now,
                IsLocked = false,
                FailedLoginCount = 0,
                UserMustChangePassword = true,
                IsEmailConfirmed = true,
                NewsSubscriptionType = NewsSubscriptionTypes.None.ToString(),
                HasFishLawConfirmation = true,
                HasTermsAgreementConfirmation = true,
                Position = user.Position
            };

            if (user.TerritoryUnitId.HasValue)
            {
                int sectorId = this.GetSectorIdByTerritoryUnit(user.TerritoryUnitId.Value);

                dbUser.UserInfo.TerritoryUnitId = user.TerritoryUnitId;
                dbUser.UserInfo.SectorId = sectorId;
                dbUser.UserInfo.DepartmentId = this.GetDepartmentIdBySector(sectorId);
            }

            if (user.MobileDevices != null)
            {
                foreach (MobileDeviceDTO device in user.MobileDevices)
                {
                    dbUser.UserMobileDevices.Add(new UserMobileDevice()
                    {
                        Imei = device.IMEI,
                        IsActive = true,
                        RequestAccessDate = now,
                        AccessStatus = UserMobileDeviceAccessStatusEnum.Approved.ToString()
                    });
                }
            }

            this.АddOrEditUserRoles(user.UserRoles, dbUser);
            this.Db.Users.Add(dbUser);
            this.Db.SaveChanges();

            this.securityEmailSender.EnqueuePasswordEmail(dbUser.Email, SecurityEmailTypes.NEW_INTERNAL_USER_MAIL);
        }

        public void AddOrEditInternalUserMobileDevices(int userId, List<MobileDeviceDTO> devices)
        {
            this.AddOrEditInternalUserDevices(userId, devices);
            this.Db.SaveChanges();
        }

        public void Delete(int id)
        {
            this.DeleteValidityRecordWithId(this.Db.Users, id);
            this.Db.SaveChanges();
        }

        public void EditExternalUser(ExternalUserDTO user)
        {
            DateTime now = DateTime.Now;

            RegixPersonDataDTO regixPersonData = new RegixPersonDataDTO()
            {
                Email = user.Email,
                EgnLnc = user.EgnLnc,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Phone = user.Phone
            };

            User dbUser = (from u in this.Db.Users
                           where u.Id == user.Id
                           select u).First();

            UserInfo dbUserInfo = (from uInfo in this.Db.UserInfos
                                   where uInfo.UserId == dbUser.Id
                                   select uInfo).First();

            Person person = this.Db.AddOrEditPerson(regixPersonData, oldPersonId: dbUser.PersonId);

            dbUser.Username = user.Username;
            dbUser.Email = user.Email;
            dbUser.Person = person;
            dbUserInfo.UserMustChangePassword = user.UserMustChangePassword.Value;
            dbUserInfo.Position = user.Position;

            if (user.TerritoryUnitId.HasValue)
            {
                int sectorId = this.GetSectorIdByTerritoryUnit(user.TerritoryUnitId.Value);

                dbUser.UserInfo.TerritoryUnitId = user.TerritoryUnitId;
                dbUser.UserInfo.SectorId = sectorId;
                dbUser.UserInfo.DepartmentId = this.GetDepartmentIdBySector(sectorId);
            }

            dbUserInfo.IsLocked = user.IsLocked.Value;
            if (!user.IsLocked.Value)
            {
                dbUserInfo.LockEndDateTime = now;
                dbUserInfo.FailedLoginCount = 0;
            }

            this.АddOrEditUserRoles(user.UserRoles, dbUser);
            this.AddOrEditUserLegals(user.UserLegals, user.Id);

            this.Db.SaveChanges();
        }

        public void EditInternalUser(InternalUserDTO user)
        {
            DateTime now = DateTime.Now;

            RegixPersonDataDTO regixPersonData = new RegixPersonDataDTO()
            {
                Email = user.Email,
                EgnLnc = user.EgnLnc,
                FirstName = user.FirstName,
                LastName = user.LastName,
                MiddleName = user.MiddleName,
                Phone = user.Phone
            };

            User dbUser = (from u in this.Db.Users
                           where u.Id == user.Id
                           select u).First();

            UserInfo dbUserInfo = (from uInfo in this.Db.UserInfos
                                   where uInfo.UserId == dbUser.Id
                                   select uInfo).First();

            Person person = this.Db.AddOrEditPerson(regixPersonData, oldPersonId: dbUser.PersonId);

            dbUser.Username = user.Username;
            dbUser.Email = user.Email;
            dbUser.Person = person;
            dbUserInfo.UserMustChangePassword = user.UserMustChangePassword.Value;
            dbUserInfo.Position = user.Position;

            if (user.TerritoryUnitId.HasValue)
            {
                int sectorId = this.GetSectorIdByTerritoryUnit(user.TerritoryUnitId.Value);

                dbUserInfo.TerritoryUnitId = user.TerritoryUnitId;
                dbUserInfo.SectorId = sectorId;
                dbUserInfo.DepartmentId = this.GetDepartmentIdBySector(sectorId);
            }

            dbUserInfo.IsLocked = user.IsLocked.Value;
            if (!user.IsLocked.Value)
            {
                dbUserInfo.LockEndDateTime = now;
                dbUserInfo.FailedLoginCount = 0;
            }

            this.АddOrEditUserRoles(user.UserRoles, dbUser);
            this.AddOrEditInternalUserDevices(user.Id, user.MobileDevices);

            this.Db.SaveChanges();
        }

        public IQueryable<UserDTO> GetAll(UserManagementFilters filters, bool IsInternalUsers)
        {
            IQueryable<UserDTO> result;
            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;

                result = this.GetAllUsers(showInactive, IsInternalUsers);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? this.GetParametersFilteredUsers(filters, IsInternalUsers)
                    : this.GetFreeTextFilteredUsers(filters.FreeTextSearch, filters.ShowInactiveRecords, IsInternalUsers);
            }

            return result;
        }

        public ExternalUserDTO GetExternalUser(int id)
        {
            DateTime now = DateTime.Now;

            ExternalUserDTO user = (from u in this.Db.Users
                                    join p in this.Db.Persons on u.PersonId equals p.Id
                                    join uInfo in this.Db.UserInfos on u.Id equals uInfo.UserId
                                    where u.Id == id
                                    select new ExternalUserDTO
                                    {
                                        Id = u.Id,
                                        FirstName = p.FirstName,
                                        MiddleName = p.MiddleName,
                                        LastName = p.LastName,
                                        Username = u.Username,
                                        Email = u.Email,
                                        EgnLnc = new EgnLncDTO
                                        {
                                            EgnLnc = p.EgnLnc,
                                            IdentifierType = Enum.Parse<IdentifierTypeEnum>(p.IdentifierType)
                                        },
                                        DepartmentId = uInfo.DepartmentId,
                                        SectorId = uInfo.SectorId,
                                        TerritoryUnitId = uInfo.TerritoryUnitId,
                                        PersonId = u.PersonId,
                                        UserMustChangePassword = uInfo.UserMustChangePassword,
                                        IsLocked = uInfo.IsLocked,
                                        Position = uInfo.Position
                                    }).First();

            string phone = this.personService.GetPersonPhoneNumber(user.PersonId);
            user.Phone = phone;

            user.UserRoles = this.GetUserRoles(user.Id);

            user.UserLegals = (from uLegal in this.Db.UserLegals
                               join uLegalCurrent in this.Db.Legals on uLegal.LegalId equals uLegalCurrent.Id
                               where uLegal.UserId == user.Id
                               select new UserLegalDTO
                               {
                                   LegalId = uLegal.LegalId,
                                   Name = uLegalCurrent.Name,
                                   EIK = uLegalCurrent.Eik,
                                   RoleId = uLegal.RoleId,
                                   Status = Enum.Parse<UserLegalStatusEnum>(uLegal.Status),
                                   IsActive = uLegal.IsActive
                               }).ToList();

            return user;
        }

        public InternalUserDTO GetInternalUser(int id)
        {
            DateTime now = DateTime.Now;

            InternalUserDTO user = (from u in this.Db.Users
                                    join p in this.Db.Persons on u.PersonId equals p.Id
                                    join uInfo in this.Db.UserInfos on u.Id equals uInfo.UserId
                                    where u.Id == id && u.IsInternalUser
                                    select new InternalUserDTO
                                    {
                                        Id = u.Id,
                                        DepartmentId = uInfo.DepartmentId,
                                        SectorId = uInfo.SectorId,
                                        TerritoryUnitId = uInfo.TerritoryUnitId,
                                        FirstName = p.FirstName,
                                        MiddleName = p.MiddleName,
                                        LastName = p.LastName,
                                        Username = u.Username,
                                        Email = u.Email,
                                        EgnLnc = new EgnLncDTO
                                        {
                                            EgnLnc = p.EgnLnc,
                                            IdentifierType = Enum.Parse<IdentifierTypeEnum>(p.IdentifierType)
                                        },
                                        PersonId = u.PersonId,
                                        UserMustChangePassword = uInfo.UserMustChangePassword,
                                        IsLocked = uInfo.IsLocked,
                                        Position = uInfo.Position
                                    }).Single();

            string phone = this.personService.GetPersonPhoneNumber(user.PersonId);
            user.Phone = phone;

            user.UserRoles = this.GetUserRoles(user.Id);
            user.MobileDevices = this.GetUserMobileDevices(id);

            return user;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return base.GetSimpleEntityAuditValues(this.Db.Users, id);
        }

        public List<MobileDeviceDTO> GetUserMobileDevices(int id)
        {
            List<MobileDeviceDTO> result = (from uMobileDevice in this.Db.UserMobileDevices
                                            where uMobileDevice.UserId == id
                                            select new MobileDeviceDTO
                                            {
                                                Id = uMobileDevice.Id,
                                                IMEI = uMobileDevice.Imei,
                                                Description = $"{uMobileDevice.Osversion} {uMobileDevice.DeviceModel} {uMobileDevice.DeviceType}",
                                                AccessStatus = uMobileDevice.AccessStatus,
                                                RequestAccessDate = uMobileDevice.RequestAccessDate
                                            }).ToList();

            return result;
        }

        public void SendChangePasswordEmail(int userId)
        {
            User user = (from u in this.Db.Users
                         where u.Id == userId
                         select u).First();

            this.securityEmailSender.EnqueuePasswordEmail(user.Email, SecurityEmailTypes.ADMIN_RESET_PASS_MAIL);
        }

        public void UndoDelete(int id)
        {
            this.UndoDeleteValidityRecordWithId(this.Db.Users, id);
            this.Db.SaveChanges();
        }

        public void AddOrEditPublicUserDevice(int userId, bool isInspector, PublicMobileDeviceDTO device)
        {
            DateTime now = DateTime.Now;

            UserMobileDevice existingMobileDevice = (
                from uDevice in this.Db.UserMobileDevices
                where uDevice.UserId == userId && uDevice.Imei == device.Imei
                select uDevice
            ).SingleOrDefault();

            if (!string.IsNullOrEmpty(device.FirebaseTokenKey))
            {
                UserMobileDevice anotherUserWithSameToken = (
                    from uDevice in this.Db.UserMobileDevices
                    where uDevice.UserId != userId && uDevice.FirebaseTokenKey == device.FirebaseTokenKey
                    select uDevice
                ).SingleOrDefault();

                if (anotherUserWithSameToken != null)
                {
                    anotherUserWithSameToken.FirebaseTokenKey = null;
                }
            }

            if (existingMobileDevice != null)
            {
                existingMobileDevice.LastLoginDate = device.LastLoginDate;
                existingMobileDevice.FirebaseTokenKey = device.FirebaseTokenKey;
                existingMobileDevice.Osversion = device.Osversion;
                existingMobileDevice.AppVersion = device.AppVersion;
            }
            else
            {
                this.Db.UserMobileDevices.Add(new UserMobileDevice()
                {
                    UserId = userId,
                    Imei = device.Imei,
                    AppVersion = device.AppVersion,
                    DeviceType = device.DeviceType,
                    LastLoginDate = device.LastLoginDate,
                    FirebaseTokenKey = device.FirebaseTokenKey,
                    Osversion = device.Osversion,
                    DeviceModel = device.DeviceModel,
                    IsActive = true,
                    RequestAccessDate = now,

                    // Това е тука понеже често се налага мобилното приложение да ги изтрием от таблета.
                    // При всяко триене трябва да отидем и да удобрим приложението наново.
#if DEBUG
                    AccessStatus = nameof(UserMobileDeviceAccessStatusEnum.Approved),
#else
                    AccessStatus = isInspector
                        ? nameof(UserMobileDeviceAccessStatusEnum.Requested)
                        : nameof(UserMobileDeviceAccessStatusEnum.Approved),
#endif
                });
            }

            this.Db.SaveChanges();
        }

        public void ChangeUserToInternal(int userId)
        {
            DateTime now = DateTime.Now;

            User dbUser = (from u in this.Db.Users
                           where u.Id == userId
                           select u).First();

            dbUser.IsInternalUser = true;
            this.Db.SaveChanges();
        }

        private void АddOrEditUserRoles(List<RoleDTO> userRoles, User user)
        {
            DateTime now = DateTime.Now;

            List<int> userRoleIDs = userRoles.Where(x => x.UserRoleId > 0).Select(x => x.UserRoleId).ToList();

            List<UserRole> existingRoles = (from userRole in this.Db.UserRoles
                                            where userRole.UserId == user.Id
                                            select userRole).ToList();

            foreach (UserRole dbRole in existingRoles)
            {
                RoleDTO role = userRoles.Where(x => x.UserRoleId == dbRole.Id).FirstOrDefault();
                if (role != null)
                {
                    dbRole.RoleId = role.Id;
                    dbRole.IsActive = role.IsActive;
                    dbRole.AccessValidFrom = role.AccessValidFrom;
                    dbRole.AccessValidTo = role.AccessValidTo;
                }
            }

            foreach (RoleDTO newRole in userRoles.Where(x => x.UserRoleId == 0))
            {
                this.Db.UserRoles.Add(new UserRole()
                {
                    User = user,
                    RoleId = newRole.Id,
                    AccessValidFrom = newRole.AccessValidFrom,
                    AccessValidTo = newRole.AccessValidTo,
                    IsActive = true
                });
            }
        }

        private void AddOrEditInternalUserDevices(int userId, List<MobileDeviceDTO> devices)
        {
            DateTime now = DateTime.Now;

            foreach (MobileDeviceDTO device in devices)
            {
                UserMobileDevice existingMobileDevice = (from uDevice in this.Db.UserMobileDevices
                                                         where device.Id == uDevice.Id
                                                         select uDevice).FirstOrDefault();

                if (existingMobileDevice != null)
                {
                    existingMobileDevice.AccessStatus = device.AccessStatus;
                }
                else
                {
                    this.Db.UserMobileDevices.Add(new UserMobileDevice()
                    {
                        UserId = userId,
                        Imei = device.IMEI,
                        IsActive = true,
                        RequestAccessDate = now,
                        AccessStatus = UserMobileDeviceAccessStatusEnum.Approved.ToString()
                    });
                }
            }
        }

        private void AddOrEditUserLegals(List<UserLegalDTO> legals, int userId)
        {
            foreach (UserLegalDTO legal in legals)
            {
                UserLegal userLegal = (from ulegal in this.Db.UserLegals
                                       where ulegal.LegalId == legal.LegalId && ulegal.UserId == userId
                                       select ulegal).SingleOrDefault();

                if (userLegal != null)
                {
                    userLegal.Status = legal.Status.ToString();
                    userLegal.RoleId = legal.RoleId;
                    userLegal.IsActive = legal.IsActive;
                }
                else
                {
                    UserLegal uLegal = new UserLegal()
                    {
                        LegalId = legal.LegalId,
                        IsActive = true,
                        RoleId = legal.RoleId,
                        Status = nameof(UserLegalStatusEnum.Approved),
                        UserId = userId
                    };

                    this.Db.UserLegals.Add(uLegal);
                }
            }
        }

        private IQueryable<UserDTO> GetAllUsers(bool showInactive, bool IsInternal)
        {
            DateTime now = DateTime.Now;
            IQueryable<UserDTO> users;

            if (IsInternal)
            {
                users = (from x in this.Db.Users
                         join info in this.Db.UserInfos on x.Id equals info.UserId
                         join p in this.Db.Persons on x.PersonId equals p.Id
                         where x.IsInternalUser
                             && ((!showInactive && x.ValidFrom < now && x.ValidTo > now)
                                || (showInactive && x.ValidFrom < now && x.ValidTo < now))
                         orderby info.RegistrationDate descending
                         select new
                         {
                             User = x,
                             PersonsHists = p,
                             RegistrationDate = info.RegistrationDate,
                             ApprovedMobileDevices = (from d in this.Db.UserMobileDevices
                                                      where d.UserId == x.Id && d.AccessStatus == UserMobileDeviceAccessStatusEnum.Approved.ToString()
                                                      select d).Count(),
                             BlockedMobileDevices = (from d in this.Db.UserMobileDevices
                                                     where d.UserId == x.Id && d.AccessStatus == UserMobileDeviceAccessStatusEnum.Blocked.ToString()
                                                     select d).Count(),
                             RequestedMobileDevices = (from d in this.Db.UserMobileDevices
                                                       where d.UserId == x.Id && d.AccessStatus == UserMobileDeviceAccessStatusEnum.Requested.ToString()
                                                       select d).Count(),
                         }).Select(user => new UserDTO()
                         {
                             Id = user.User.Id,
                             Username = user.User.Username,
                             Email = user.User.Email,
                             FirstName = user.PersonsHists.FirstName,
                             MiddleName = user.PersonsHists.MiddleName,
                             LastName = user.PersonsHists.LastName,
                             IsActive = user.User.ValidTo > now && user.User.ValidFrom < now,
                             RegistrationDate = user.RegistrationDate,
                             UserRoles = string.Join(", ", from ur in this.Db.UserRoles
                                                           join r in this.Db.Roles on ur.RoleId equals r.Id
                                                           where ur.UserId == user.User.Id
                                                                 && ur.IsActive
                                                                 && ur.AccessValidFrom < now
                                                                 && ur.AccessValidTo > now
                                                           select r.Name),
                             MobileDevices = $"{(user.ApprovedMobileDevices > 0 ? AppResources.approvedUserMobileDevices + user.ApprovedMobileDevices.ToString() + "; " : "")}" +
                             $"{(user.BlockedMobileDevices > 0 ? AppResources.blockedUserMobileDevices + user.BlockedMobileDevices.ToString() + "; " : "")}" +
                             $"{(user.RequestedMobileDevices > 0 ? AppResources.requestedUserMobileDevices + user.RequestedMobileDevices.ToString() : "")}"
                         });
            }
            else
            {
                users = from x in this.Db.Users
                        join info in this.Db.UserInfos on x.Id equals info.UserId
                        join p in this.Db.Persons on x.PersonId equals p.Id
                        where !x.IsInternalUser
                          && ((!showInactive && x.ValidFrom < now && x.ValidTo > now)
                          || (showInactive && x.ValidFrom < now && x.ValidTo < now))
                        orderby info.RegistrationDate descending
                        select new UserDTO
                        {
                            Id = x.Id,
                            Username = x.Username,
                            Email = x.Email,
                            FirstName = p.FirstName,
                            MiddleName = p.MiddleName,
                            LastName = p.LastName,
                            IsActive = x.ValidTo > now && x.ValidFrom < now,
                            RegistrationDate = info.RegistrationDate,
                            UserRoles = string.Join(", ", from ur in this.Db.UserRoles
                                                          join r in this.Db.Roles on ur.RoleId equals r.Id
                                                          where ur.UserId == x.Id
                                                                && ur.IsActive
                                                                && ur.AccessValidFrom < now
                                                                && ur.AccessValidTo > now
                                                          select r.Name)
                        };
            }

            return users;
        }

        private int GetDepartmentIdBySector(int sectorId)
        {
            return (from s in this.Db.Nsectors
                    where s.Id == sectorId
                    select s.DepartmentId).First();
        }

        private IQueryable<UserDTO> GetFreeTextFilteredUsers(string text, bool showInactive, bool IsInternal)
        {
            text = text.ToLowerInvariant();
            DateTime? searchDate = DateTimeUtils.TryParseDate(text);
            DateTime now = DateTime.Now;

            IQueryable<UserDTO> result = from u in this.Db.Users
                                         join info in this.Db.UserInfos on u.Id equals info.UserId
                                         join p in this.Db.Persons on u.PersonId equals p.Id
                                         where u.IsInternalUser == IsInternal
                                            && ((!showInactive && u.ValidFrom < now && u.ValidTo > now)
                                            || (showInactive && u.ValidFrom < now && u.ValidTo < now))
                                            && (u.Username.ToLower().Contains(text)
                                                || p.FirstName.ToLower().Contains(text)
                                                || p.MiddleName.ToLower().Contains(text)
                                                || p.LastName.ToLower().Contains(text)
                                                || u.Email.ToLower().Contains(text)
                                                || u.ValidFrom.Date == searchDate)
                                         orderby info.RegistrationDate descending
                                         select new UserDTO
                                         {
                                             Id = u.Id,
                                             Username = u.Username,
                                             FirstName = p.FirstName,
                                             MiddleName = p.MiddleName,
                                             LastName = p.LastName,
                                             Email = u.Email,
                                             IsActive = u.ValidFrom < now && u.ValidTo > now,
                                             RegistrationDate = info.RegistrationDate,
                                             UserRoles = string.Join(", ", from ur in this.Db.UserRoles
                                                                           join r in this.Db.Roles on ur.RoleId equals r.Id
                                                                           where ur.UserId == u.Id
                                                                                 && ur.IsActive
                                                                                 && ur.AccessValidFrom < now
                                                                                 && ur.AccessValidTo > now
                                                                           select r.Name)
                                         };

            return result;
        }

        private IQueryable<UserDTO> GetParametersFilteredUsers(UserManagementFilters filters, bool IsInternal)
        {
            DateTime now = DateTime.Now;

            var query = from u in this.Db.Users
                        join info in this.Db.UserInfos on u.Id equals info.UserId
                        join p in this.Db.Persons on u.PersonId equals p.Id
                        where u.IsInternalUser == IsInternal
                            && ((!filters.ShowInactiveRecords && u.ValidFrom < now && u.ValidTo > now)
                                || (filters.ShowInactiveRecords && u.ValidFrom < now && u.ValidTo < now))
                        select new
                        {
                            u.Id,
                            u.Username,
                            p.FirstName,
                            p.MiddleName,
                            p.LastName,
                            u.Email,
                            IsActive = u.ValidFrom < now && u.ValidTo > now,
                            u.IsInternalUser,
                            u.ValidFrom,
                            u.UserRoles,
                            u.UserMobileDevices,
                            info.RegistrationDate
                        };

            if (filters.RoleId.HasValue)
            {
                query = from u in query
                        join ur in this.Db.UserRoles on u.Id equals ur.UserId
                        where ur.RoleId == filters.RoleId
                               && ur.IsActive
                               && ur.AccessValidFrom < now
                               && ur.AccessValidTo > now
                        select u;
            }

            if (filters.IsRequestedAccess == true)
            {
                query = (from u in query
                         join uDevice in this.Db.UserMobileDevices on u.Id equals uDevice.UserId
                         where uDevice.AccessStatus == UserMobileDeviceAccessStatusEnum.Requested.ToString()
                         select u);
            }

            if (!string.IsNullOrEmpty(filters.Username))
            {
                query = query.Where(x => x.Username.ToLower().Contains(filters.Username.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.FirstName))
            {
                query = query.Where(x => x.FirstName.ToLower().Contains(filters.FirstName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.MiddleName))
            {
                query = query.Where(x => x.MiddleName.ToLower().Contains(filters.MiddleName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.LastName))
            {
                query = query.Where(x => x.LastName.ToLower().Contains(filters.LastName.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.Email))
            {
                query = query.Where(x => x.Email.ToLower().Contains(filters.Email.ToLower()));
            }

            if (filters.RegisteredDateFrom.HasValue)
            {
                query = query.Where(x => x.RegistrationDate >= filters.RegisteredDateFrom);
            }

            if (filters.RegisteredDateTo.HasValue)
            {
                query = query.Where(x => x.RegistrationDate <= filters.RegisteredDateTo);
            }

            IQueryable<UserDTO> result;

            if (IsInternal)
            {
                result = (from u in query
                          orderby u.RegistrationDate descending
                          select new
                          {
                              u.Id,
                              u.Username,
                              u.FirstName,
                              u.MiddleName,
                              u.LastName,
                              u.Email,
                              u.IsActive,
                              u.IsInternalUser,
                              u.ValidFrom,
                              u.UserRoles,
                              u.RegistrationDate,
                              approvedDevices = u.UserMobileDevices.Where(x => x.AccessStatus == UserMobileDeviceAccessStatusEnum.Approved.ToString()).Count(),
                              blockedDevices = u.UserMobileDevices.Where(x => x.AccessStatus == UserMobileDeviceAccessStatusEnum.Blocked.ToString()).Count(),
                              requestedDevices = u.UserMobileDevices.Where(x => x.AccessStatus == UserMobileDeviceAccessStatusEnum.Requested.ToString()).Count(),
                          }).Select(user => new UserDTO()
                          {
                              Id = user.Id,
                              Username = user.Username,
                              Email = user.Email,
                              FirstName = user.FirstName,
                              MiddleName = user.MiddleName,
                              LastName = user.LastName,
                              IsActive = user.IsActive,
                              RegistrationDate = user.RegistrationDate,
                              UserRoles = string.Join(", ", from ur in this.Db.UserRoles
                                                            join r in this.Db.Roles on ur.RoleId equals r.Id
                                                            where ur.UserId == user.Id
                                                               && ur.IsActive
                                                               && ur.AccessValidFrom < now
                                                               && ur.AccessValidTo > now
                                                            select r.Name),
                              MobileDevices = $"{(user.approvedDevices > 0 ? AppResources.approvedUserMobileDevices + user.approvedDevices.ToString() + "; " : "")}" +
                             $"{(user.blockedDevices > 0 ? AppResources.blockedUserMobileDevices + user.blockedDevices.ToString() + "; " : "")}" +
                             $"{(user.requestedDevices > 0 ? AppResources.requestedUserMobileDevices + user.requestedDevices.ToString() : "")}"
                          });
            }
            else
            {
                result = from u in query
                         orderby u.RegistrationDate descending
                         select new UserDTO
                         {
                             Id = u.Id,
                             Username = u.Username,
                             FirstName = u.FirstName,
                             MiddleName = u.MiddleName,
                             LastName = u.LastName,
                             Email = u.Email,
                             IsActive = u.IsActive,
                             RegistrationDate = u.RegistrationDate,
                             UserRoles = string.Join(", ", from ur in this.Db.UserRoles
                                                           join r in this.Db.Roles on ur.RoleId equals r.Id
                                                           where ur.UserId == u.Id
                                                              && ur.IsActive
                                                              && ur.AccessValidFrom < now
                                                              && ur.AccessValidTo > now
                                                           select r.Name)
                         };
            }

            return result;
        }

        private int GetSectorIdByTerritoryUnit(int territoryUnitId)
        {
            return (from tu in this.Db.NterritoryUnits
                    where tu.Id == territoryUnitId
                    select tu.SectorId).First();
        }

        private List<RoleDTO> GetUserRoles(int userId)
        {
            return (from ur in this.Db.UserRoles
                    join r in this.Db.Roles on ur.RoleId equals r.Id
                    where ur.UserId == userId
                    select new RoleDTO
                    {
                        Id = ur.RoleId,
                        UserRoleId = ur.Id,
                        Name = r.Name,
                        AccessValidFrom = ur.AccessValidFrom,
                        AccessValidTo = ur.AccessValidTo,
                        IsActive = ur.IsActive
                    }).ToList();
        }
    }
}
