using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IARA.Common.Constants;
using IARA.EntityModels.Entities;
using IARA.Logging.Abstractions.Models;
using IARA.Security.AuthContext;
using IARA.Security.Enum;
using IARA.Security.SecurityModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace IARA.Security.Services
{
    public class SecurityService : ISecurityService
    {
        private AuthDbContext Db;

        private const int MAX_FAILED_LOGIN_COUNT = 6;
        private const int LOCKOUT_HOURS = 1;

        public SecurityService(AuthDbContext db)
        {
            this.Db = db;
        }

        public List<int> GetPermissions(List<int> roleIds)
        {
            return (from x in Db.RolePermissions
                    join y in Db.Npermissions on x.PermissionId equals y.Id
                    where roleIds.Contains(x.RoleId)
                       && x.IsActive
                       && y.ValidFrom < DateTime.Now && y.ValidTo > DateTime.Now
                    select y.Id).Distinct().ToList();
        }


        public SecurityUser GetInternalUser(string identifier, bool searchByPersonId)
        {
            identifier = identifier.ToLower();
            if (searchByPersonId)
            {
                return (from x in this.Db.Users
                        join person in this.Db.Persons on x.PersonId equals person.Id
                        join info in this.Db.UserInfos on x.Id equals info.UserId
                        where person.EgnLnc == identifier && x.IsInternalUser
                        select new SecurityUser
                        {
                            Id = x.Id,
                            Email = x.Email,
                            UserName = x.Username,
                            AccessFailedCount = info.FailedLoginCount,
                            EmailConfirmed = info.IsEmailConfirmed,
                            PasswordHash = x.Password,
                            HasEauthLogin = x.HasEauthLogin,
                            HasUserPassLogin = x.HasUserPassLogin,
                            IsInternalUser = x.IsInternalUser,
                            PersonId = x.PersonId,
                            IsLocked = info.IsLocked,
                            LockEndDateTime = info.LockEndDateTime,
                            UserMustChangePassword = info.UserMustChangePassword,
                            ValidFrom = x.ValidFrom,
                            ValidTo = x.ValidTo
                        }).FirstOrDefault();
            }
            else
            {
                return (from x in this.Db.Users
                        join info in this.Db.UserInfos on x.Id equals info.UserId
                        where x.Username.ToLower() == identifier && x.IsInternalUser
                        select new SecurityUser
                        {
                            Id = x.Id,
                            Email = x.Email,
                            UserName = x.Username,
                            AccessFailedCount = info.FailedLoginCount,
                            EmailConfirmed = info.IsEmailConfirmed,
                            PasswordHash = x.Password,
                            HasEauthLogin = x.HasEauthLogin,
                            HasUserPassLogin = x.HasUserPassLogin,
                            IsInternalUser = x.IsInternalUser,
                            PersonId = x.PersonId,
                            IsLocked = info.IsLocked,
                            LockEndDateTime = info.LockEndDateTime,
                            UserMustChangePassword = info.UserMustChangePassword,
                            ValidFrom = x.ValidFrom,
                            ValidTo = x.ValidTo
                        }).FirstOrDefault();
            }
        }

        public SecurityUser GetUser(int userId)
        {
            return (from x in Db.Users
                    join info in Db.UserInfos on x.Id equals info.UserId
                    where x.Id == userId
                    select new SecurityUser
                    {
                        Id = x.Id,
                        Email = x.Email,
                        UserName = x.Username,
                        AccessFailedCount = info.FailedLoginCount,
                        EmailConfirmed = info.IsEmailConfirmed,
                        PasswordHash = x.Password,
                        HasEauthLogin = x.HasEauthLogin,
                        HasUserPassLogin = x.HasUserPassLogin,
                        IsInternalUser = x.IsInternalUser,
                        PersonId = x.PersonId,
                        IsLocked = info.IsLocked,
                        LockEndDateTime = info.LockEndDateTime,
                        UserMustChangePassword = info.UserMustChangePassword,
                        ValidFrom = x.ValidFrom,
                        ValidTo = x.ValidTo
                    }).FirstOrDefault();
        }

        public SecurityUser GetUser(string identifier, bool searchByPersonId)
        {
            if (searchByPersonId)
            {
                return (from x in Db.Users
                        join person in this.Db.Persons on x.PersonId equals person.Id
                        join info in Db.UserInfos on x.Id equals info.UserId
                        where person.EgnLnc == identifier
                        select new SecurityUser
                        {
                            Id = x.Id,
                            Email = x.Email,
                            UserName = x.Username,
                            AccessFailedCount = info.FailedLoginCount,
                            EmailConfirmed = info.IsEmailConfirmed,
                            PasswordHash = x.Password,
                            HasEauthLogin = x.HasEauthLogin,
                            HasUserPassLogin = x.HasUserPassLogin,
                            IsInternalUser = x.IsInternalUser,
                            PersonId = x.PersonId,
                            IsLocked = info.IsLocked,
                            LockEndDateTime = info.LockEndDateTime,
                            UserMustChangePassword = info.UserMustChangePassword,
                            ValidFrom = x.ValidFrom,
                            ValidTo = x.ValidTo
                        }).FirstOrDefault();
            }
            else
            {
                return (from x in Db.Users
                        join info in Db.UserInfos on x.Id equals info.UserId
                        where x.Username == identifier
                        select new SecurityUser
                        {
                            Id = x.Id,
                            Email = x.Email,
                            UserName = x.Username,
                            AccessFailedCount = info.FailedLoginCount,
                            EmailConfirmed = info.IsEmailConfirmed,
                            PasswordHash = x.Password,
                            HasEauthLogin = x.HasEauthLogin,
                            HasUserPassLogin = x.HasUserPassLogin,
                            IsInternalUser = x.IsInternalUser,
                            PersonId = x.PersonId,
                            IsLocked = info.IsLocked,
                            LockEndDateTime = info.LockEndDateTime,
                            UserMustChangePassword = info.UserMustChangePassword,
                            ValidFrom = x.ValidFrom,
                            ValidTo = x.ValidTo
                        }).FirstOrDefault();
            }
        }

        public SecurityUser GetUserByPersonIdentifier(string egnLnch)
        {
            return (from user in Db.Users
                    join person in this.Db.Persons on user.PersonId equals person.Id
                    join info in Db.UserInfos on user.Id equals info.UserId
                    where person.EgnLnc == egnLnch
                    select new SecurityUser
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.Username,
                        AccessFailedCount = info.FailedLoginCount,
                        EmailConfirmed = info.IsEmailConfirmed,
                        PasswordHash = user.Password,
                        HasEauthLogin = user.HasEauthLogin,
                        HasUserPassLogin = user.HasUserPassLogin,
                        IsInternalUser = user.IsInternalUser,
                        PersonId = user.PersonId,
                        IsLocked = info.IsLocked,
                        LockEndDateTime = info.LockEndDateTime,
                        UserMustChangePassword = info.UserMustChangePassword,
                        ValidFrom = user.ValidFrom,
                        ValidTo = user.ValidTo
                    }).FirstOrDefault();
        }

        public SecurityUser GetUserByVATNumber(string vatNumber)
        {
            return (from legal in Db.Legals
                    join person in Db.Persons on legal.LegalOwnerId equals person.Id
                    join usr in Db.Users on person.Id equals usr.PersonId
                    join info in Db.UserInfos on usr.Id equals info.UserId
                    where legal.Eik == vatNumber && legal.LegalOwnerId.HasValue
                    select new SecurityUser
                    {
                        Id = usr.Id,
                        Email = usr.Email,
                        UserName = usr.Username,
                        AccessFailedCount = info.FailedLoginCount,
                        EmailConfirmed = info.IsEmailConfirmed,
                        PasswordHash = usr.Password,
                        HasEauthLogin = usr.HasEauthLogin,
                        HasUserPassLogin = usr.HasUserPassLogin,
                        IsInternalUser = usr.IsInternalUser,
                        PersonId = usr.PersonId,
                        IsLocked = info.IsLocked,
                        LockEndDateTime = info.LockEndDateTime,
                        UserMustChangePassword = info.UserMustChangePassword,
                        ValidFrom = usr.ValidFrom,
                        ValidTo = usr.ValidTo
                    }).FirstOrDefault();
        }

        public void UpdateUserRoles(int userId, LoginTypesEnum loginType, bool isFromLegal = false)
        {
            string roleCode = null;

            switch (loginType)
            {
                case LoginTypesEnum.PASSWORD:
                    {
                        roleCode = "Person_Email";
                    }
                    break;
                case LoginTypesEnum.EAUTH:
                    {
                        roleCode = "Person_KEP";
                    }
                    break;
            }

            if (!HasUserRole(userId, roleCode, out int roleId))
            {
                if (roleId != 0)
                {
                    Db.UserRoles.Add(new UserRole
                    {
                        UserId = userId,
                        RoleId = roleId,
                        IsActive = true,
                        AccessValidFrom = DefaultConstants.MIN_VALID_DATE,
                        AccessValidTo = DefaultConstants.MAX_VALID_DATE
                    });
                }
            }

            Db.SaveChanges();
        }

        public List<int> GetUserLegalRoles(int userId)
        {
            var now = DateTime.Now;

            return (from x in Db.UserLegals
                    join y in Db.Legals on x.LegalId equals y.Id
                    where x.UserId == userId
                       && x.AccessValidFrom < now && x.AccessValidTo > now
                       && x.IsActive
                       && y.ValidFrom < now && y.ValidTo > now
                    select x.RoleId).ToList();
        }

        public List<int> GetUserRoles(int userId)
        {
            var now = DateTime.Now;

            return (from x in Db.UserRoles
                    join y in Db.Roles on x.RoleId equals y.Id
                    where x.UserId == userId
                       && x.AccessValidFrom < now && x.AccessValidTo > now
                       && y.ValidFrom < now && y.ValidTo > now
                       && x.IsActive
                    select y.Id).ToList();
        }

        public bool IsActive(int userId)
        {
            SecurityUser user = GetUser(userId);
            return IsActive(user);
        }

        public void UpdateUser(SecurityUser user)
        {
            var dbUser = (from x in Db.Users
                          where x.Id == user.Id
                          select x).First();

            var dbUserInfo = (from x in Db.UserInfos
                              where x.UserId == user.Id
                              select x).First();


            dbUser.HasEauthLogin = user.HasEauthLogin;
            dbUser.HasUserPassLogin = user.HasUserPassLogin;
            dbUser.Password = user.PasswordHash;
            dbUser.ValidFrom = user.ValidFrom;
            dbUser.ValidTo = user.ValidTo;
            dbUser.PersonId = user.PersonId;
            dbUserInfo.UserMustChangePassword = user.UserMustChangePassword;
            dbUserInfo.FailedLoginCount = user.AccessFailedCount;
            dbUser.IsInternalUser = user.IsInternalUser;
            dbUserInfo.IsEmailConfirmed = user.EmailConfirmed;


            if (user.AccessFailedCount >= MAX_FAILED_LOGIN_COUNT)
            {
                user.IsLocked = true;
                dbUserInfo.IsLocked = user.IsLocked;
                dbUserInfo.LockEndDateTime = DateTime.Now.AddHours(LOCKOUT_HOURS);
            }

            Db.SaveChanges();
        }

        public bool IsActive(SecurityUser user)
        {
            var now = DateTime.Now;

            return user.EmailConfirmed
                && (!user.IsLocked || (user.IsLocked && user.LockEndDateTime.HasValue && user.LockEndDateTime.Value < now))
                && user.ValidFrom <= now && user.ValidTo >= now;
        }

        public RequestData GetRequestContext(HttpContext httpContext)
        {
            IPAddress remoteIp = httpContext.Connection.RemoteIpAddress;
            IHeaderDictionary headers = httpContext.Request.Headers;
            string ip = headers.TryGetValue(DefaultConstants.FORWARDED_FOR, out StringValues forwarded) ? forwarded.ToString() : remoteIp.ToString();
            string endpoint = httpContext.Request.Path.ToUriComponent();
            string browserInfo = headers.TryGetValue(DefaultConstants.USER_AGENT, out StringValues userAgent) ? userAgent.ToString() : string.Empty;

            return new RequestData
            {
                BrowserInfo = browserInfo,
                Endpoint = endpoint,
                IPAddress = ip,
                TimeOfRequest = DateTime.Now
            };
        }

        public async Task<int> LogUserLoginAction(HttpContext context, bool isLogin, bool? isSuccessful = null, string tableId = null, string userName = null)
        {
            RequestData requestData = GetRequestContext(context);

            if (string.IsNullOrEmpty(userName))
            {
                userName = context?.User?.Identity.Name;
                if (!string.IsNullOrEmpty(userName))
                {
                    var user = GetUser(userName, false);

                    if (user == null) return -1;

                    tableId = user.Id.ToString();
                    userName = user.UserName;
                }
            }

            string[] sections = context.Request.Path.ToUriComponent().Split("/", StringSplitOptions.RemoveEmptyEntries);

            Db.AuditLogs.Add(new AuditLog
            {
                BrowserInfo = requestData.BrowserInfo,
                Ipaddress = requestData.IPAddress,
                LogDate = DateTime.Now,
                Action = sections[sections.Length - 1],
                Application = string.Join("/", sections.Take(sections.Length - 1)),
                ActionType = isLogin ? "LOGIN" : "LOGOUT",
                NewValue = isLogin ? ((isSuccessful != null && isSuccessful.HasValue && isSuccessful.Value) ? "SUCCESSFUL" : "FAILED") : "LOGOUT",
                OldValue = isLogin ? "LOGIN" : "LOGOUT",
                SchemaName = "UsrMgmt",
                TableId = !string.IsNullOrEmpty(tableId) ? tableId : "0",
                TableName = "Users",
                Username = !string.IsNullOrEmpty(userName) ? userName : "SYSTEM"
            });

            int result = await Db.SaveChangesAsync();

            return result;
        }


        private bool HasUserRole(int userId, string roleCode, out int roleId)
        {
            roleId = (from user in Db.Users
                      join userRole in Db.UserRoles on user.Id equals userRole.UserId
                      join role in Db.Roles on userRole.RoleId equals role.Id
                      where user.Id == userId && role.Code == roleCode
                      select role.Id).FirstOrDefault();

            bool result = roleId != 0;

            if (!result)
            {
                roleId = Db.Roles.Where(x => x.Code == roleCode).Select(x => x.Id).FirstOrDefault();
            }

            return result;
        }
    }
}
