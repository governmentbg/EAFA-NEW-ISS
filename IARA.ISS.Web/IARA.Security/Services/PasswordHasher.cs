using IARA.Common.Utils;
using IARA.Security.SecurityModels;
using Microsoft.AspNetCore.Identity;

namespace IARA.Security
{
    public class PasswordHasher : IPasswordHasher<SecurityUser>
    {
        public string HashPassword(SecurityUser user, string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                string hashedPassword = CommonUtils.GetPasswordHash(password, user.Email);
                user.PasswordHash = hashedPassword;
                return hashedPassword;
            }
            else
            {
                return null;
            }
        }

        public PasswordVerificationResult VerifyHashedPassword(SecurityUser user, string hashedPassword, string providedPassword)
        {
            if (hashedPassword == HashPassword(user, providedPassword))
            {
                return PasswordVerificationResult.Success;
            }
            else
            {
                return PasswordVerificationResult.Failed;
            }
        }
    }
}
