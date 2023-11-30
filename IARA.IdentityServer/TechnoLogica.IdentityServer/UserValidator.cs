using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TechnoLogica.Authentication.Common;

namespace TechnoLogica.IdentityServer
{
    public class UserValidator
    {
        IEnumerable<IProfileClientService> ProfileClientServices { get; set; }

        public UserValidator(IEnumerable<IProfileClientService> profileClientServices)
        {
            ProfileClientServices = profileClientServices;
        }

        public async Task<bool> ValidateCredentials(string scheme, string clientID, string username, string password)
        {
            var profileService = ProfileClientServices.Where(cs => cs.ClientId.Equals(clientID)).FirstOrDefault();
            if (profileService != null)
            {
                var result = await profileService.ValidateCredentials(scheme, username, password);
                return result;
            }
            else
            {
                return false;
            }
        }

        public async Task<UserInfo> FindByUsername(string scheme, string clientID, string username, List<Claim> externalClaims)
        {
            var profileService =
                //await 
                ProfileClientServices.Where(cs => cs.ClientId.Equals(clientID))
                //.ToAsyncEnumerable()
                .FirstOrDefault();
            if (profileService != null)
            {
                var result = await profileService.FindByUsername(scheme, username, externalClaims);
                return result;
            }
            else
            {
                return null;
            }
        }

        public async Task<bool> ChangePassword(string scheme, string clientID, string username, string password, string newPassword)
        {
            var profileService =
                //await 
                ProfileClientServices.Where(cs => cs.ClientId.Equals(clientID))
                //.ToAsyncEnumerable()
                .FirstOrDefault();
            if (profileService != null)
            {
                var result = await profileService.ChangePassword(scheme, username, password, newPassword);
                return result;
            }
            else
            {
                return false;
            }
        }

        public async Task<ResetPasswordResult> ResetPassword(string scheme, string clientID, string email, string token, string password)
        {
            var profileService =
                ProfileClientServices.Where(cs => cs.ClientId.Equals(clientID))
                .FirstOrDefault();
            if (profileService != null)
            {
                var result = await profileService.ResetPasswordAsync(scheme, email, token, password);
                return result;
            }
            else
            {
                return new ResetPasswordResult() { Succeeded = false };
            }
        }

        public async Task<bool> SendPasswordResetToken(string scheme, string clientID, string baseAddress, string email)
        {
            var profileService =
                ProfileClientServices.Where(cs => cs.ClientId.Equals(clientID))
                .FirstOrDefault();
            if (profileService != null)
            {
                var result = await profileService.SendPasswordResetTokenAsync(scheme, baseAddress, email);
                return result;
            }
            else
            {
                return false;
            }
        }

        public async Task<UserRegistrationResult> RegisterUser(string scheme, string clientID, string name, string userName, string email, string password, Dictionary<string, string> additionalAttributes)
        {
            var profileService =
                //await 
                ProfileClientServices.Where(cs => cs.ClientId.Equals(clientID))
                //.ToAsyncEnumerable()
                .FirstOrDefault();
            if (profileService != null)
            {
                return await profileService.RegisterUser(scheme, name, userName, email, password, additionalAttributes);
            }
            else
            {
                return
                    new UserRegistrationResult()
                    {
                        Succeeded = false,
                        Errors = new List<UserRegistrationError>() {
                            new UserRegistrationError() {
                                Code = "NO_PROFILE",
                                Description = "Client profile not found"
                            }
                        }
                    };
            }
        }

        public async Task SignOutAsync(string clientID)
        {
            var profileService =
                //await 
                ProfileClientServices.Where(cs => cs.ClientId.Equals(clientID))
                //.ToAsyncEnumerable()
                .FirstOrDefault();
            if (profileService != null)
            {
                await profileService.SignOutAsync();
            }
        }
    }
}
