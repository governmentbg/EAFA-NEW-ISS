using IARA.Security.AuthContext;
using TL.AspNet.Security.Abstractions.Models;
using TL.AspNet.Security.Abstractions.Stores;

namespace IARA.Infrastructure.Services.Users
{
    public class UserLoginHistoryStore : IUserLoginHistoryStore
    {
        private RequestContext<string> requestContext;
        private AuthDbContext Db;
        public UserLoginHistoryStore(RequestContext<string> context, AuthDbContext db)
        {
            this.requestContext = context;
            this.Db = db;
        }

        public void StoreUserLoginAttempt(string username, bool success, string userAddress)
        {
            int userId = (from u in Db.Users
                          where u.Username == username
                          select u.Id).FirstOrDefault();

            AuditLog auditLog = new AuditLog
            {
                LogDate = DateTime.Now,
                Username = username,
                IpAddress = userAddress,
                BrowserInfo = this.requestContext.BrowserInfo,
                Application = "Account",
                Action = "Login",
                ActionType = "LOGIN",
                SchemaName = "UsrMgmt",
                TableId = userId.ToString(),
                TableName = "Users",
                OldValue = "LOGIN",
                NewValue = success ? "SUCCESSFUL" : "FAILED"
            };

            this.Db.AuditLogs.Add(auditLog);

            this.Db.SaveChanges();
        }
    }
}
