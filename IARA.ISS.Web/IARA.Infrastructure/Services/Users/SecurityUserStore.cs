using IARA.Security.AuthContext;
using TL.AspNet.Security.Abstractions.Enums;
using TL.AspNet.Security.Abstractions.Services;
using TL.AspNet.Security.Abstractions.Settings;
using TL.AspNet.Security.Abstractions.Stores;
using TL.AspNet.Security.Abstractions.User;

namespace IARA.Infrastructure.Services.Users
{
    public class SecurityUserStore : IUserStore<string>,
                                     IPasswordUserStore<string>,
                                     IUserBlockStore<string>,
                                     IUserLockoutStore<string>,
                                     IEGovUserStore<string>,
                                     IEmailConfirmationStore<string>
    {

        private IPasswordHasher passwordHasher;
        private AuthDbContext Db;
        public SecurityUserStore(IPasswordHasher passwordHasher, AuthDbContext db)
        {
            this.passwordHasher = passwordHasher;
            this.Db = db;
        }

        public LoginTypes UserManagerType { get; set; }

        public IUserIdentifier<string> GetUser(string username)
        {
            SecurityUserEntity result = (from u in this.Db.Users
                                         join ui in this.Db.UserInfos on u.Id equals ui.UserId
                                         join p in this.Db.Persons on u.PersonId equals p.Id
                                         where u.Username == username
                                         select new SecurityUserEntity
                                         {
                                             Id = u.Id,
                                             Username = u.Username,
                                             Password = u.Password,
                                             Email = u.Email,
                                             PersonId = u.PersonId,
                                             LastFailedLoginAttempt = ui.LastFailedLoginAttempt,
                                             FailedLoginCount = ui.FailedLoginCount,
                                             IsLocked = ui.IsLocked,
                                             LockedUntil = ui.LockEndDateTime,
                                             LastLoginDate = ui.LastLoginDate,
                                             EmailConfirmationToken = ui.ConfirmEmailKey,
                                             IsEmailConfirmed = ui.IsEmailConfirmed,
                                             EmailKeyValidTo = ui.EmailKeyValidTo,
                                             Egn = p.EgnLnc,
                                             ValidFrom = u.ValidFrom,
                                             ValidTo = u.ValidTo,
                                         }).FirstOrDefault();

            return result;
        }

        public IUserIdentifier<string> GetUserByEgn(string egn)
        {
            SecurityUserEntity result = (from u in this.Db.Users
                                         join ui in this.Db.UserInfos on u.Id equals ui.UserId
                                         join p in this.Db.Persons on u.PersonId equals p.Id
                                         where p.EgnLnc == egn && p.ValidTo > DateTime.Now
                                         select new SecurityUserEntity
                                         {
                                             Id = u.Id,
                                             Username = u.Username,
                                             Password = u.Password,
                                             Email = u.Email,
                                             PersonId = u.PersonId,
                                             LastFailedLoginAttempt = ui.LastFailedLoginAttempt,
                                             FailedLoginCount = ui.FailedLoginCount,
                                             IsLocked = ui.IsLocked,
                                             LockedUntil = ui.LockEndDateTime,
                                             LastLoginDate = ui.LastLoginDate,
                                             EmailConfirmationToken = ui.ConfirmEmailKey,
                                             IsEmailConfirmed = ui.IsEmailConfirmed,
                                             EmailKeyValidTo = ui.EmailKeyValidTo,
                                             Egn = p.EgnLnc,
                                             ValidFrom = u.ValidFrom,
                                             ValidTo = u.ValidTo,
                                         }).FirstOrDefault();

            return result;
        }

        public IUserIdentifier<string> GetUserById(string id)
        {
            return GetUser(id);
        }

        public bool IsEmailConfirmed(IEmailConfirmationUser<string> user)
        {
            return user.IsEmailConfirmed;
        }

        public bool IsUserBlocked(IBlockUser<string> user)
        {
            return user.IsBlocked;
        }

        public void SetUserConfirmationToken(IEmailConfirmationUser<string> user, string token)
        {
            UserInfo userInfo = this.GetUserInfo(user);

            userInfo.ConfirmEmailKey = token;

            this.Db.SaveChanges();
        }

        public bool TryLockUser(ILockoutUser<string> user)
        {
            UserInfo userInfo = this.GetUserInfo(user);

            user.FailedLoginCount = user.FailedLoginCount.GetValueOrDefault() + 1;
            userInfo.FailedLoginCount = (short)user.FailedLoginCount.Value;
            userInfo.LastLoginDate = user.LastFailedLoginAttempt = DateTime.Now;

            if (user.FailedLoginCount >= SecuritySettings.Default.MaxFailedLoginCount)
            {
                user.LockedUntil = DateTime.Now + SecuritySettings.Default.LockoutTime;
                userInfo.LockEndDateTime = user.LockedUntil;
                userInfo.IsLocked = user.IsLocked = true;
            }

            this.Db.SaveChanges();

            return user.IsLocked;
        }

        public void UnlockUser(ILockoutUser<string> user)
        {
            UserInfo userInfo = this.GetUserInfo(user);

            userInfo.LastLoginDate = user.LastLoginDate = DateTime.Now;
            userInfo.LockEndDateTime = user.LockedUntil = null;
            user.FailedLoginCount = 0;
            userInfo.FailedLoginCount = 0;
            userInfo.IsLocked = user.IsLocked = false;

            this.Db.SaveChanges();
        }

        public void UpdatePassword(IPasswordUser<string> user, string password)
        {
            User dbUser = this.GetDbUser(user);

            dbUser.Password = user.Password = this.passwordHasher.HashPassword(password, dbUser.Email);

            this.Db.SaveChanges();
        }

        private User GetDbUser(IUserIdentifier<string> user)
        {
            User dbUser = (from u in this.Db.Users
                           where u.Username == user.UserIdentifier
                           select u).FirstOrDefault();

            return dbUser;
        }

        private UserInfo GetUserInfo(IUserIdentifier<string> user)
        {
            UserInfo userInfo = (from u in this.Db.Users
                                 join ui in this.Db.UserInfos on u.Id equals ui.UserId
                                 where u.Username == user.UserIdentifier
                                 select ui).FirstOrDefault();

            return userInfo;
        }
    }
}
