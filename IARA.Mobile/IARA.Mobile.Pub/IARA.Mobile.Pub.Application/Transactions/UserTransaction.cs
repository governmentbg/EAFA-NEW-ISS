using System.Threading.Tasks;
using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Application.Extensions;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.User.API;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Transactions.Base;

namespace IARA.Mobile.Pub.Application.Transactions
{
    public class UserTransaction : BaseTransaction, IUserTransaction
    {
        public UserTransaction(BaseTransactionProvider provider) : base(provider)
        {
        }

        public async Task<ResponseApiDto> AddUser(UserRegistrationDto user)
        {
            HttpResult result = await RestClient.PostAsync("User/AddExternalUser", user, "Common");

            return new ResponseApiDto
            {
                IsSuccessful = result.IsSuccessful,
                ErrorMessages = result.Error?.Type == ErrorTypeEnum.Validation ? result.Error?.Messages : null,
            };
        }

        public async Task<bool?> ConfirmEmailAndPassword(ConfirmUserDataDto user)
        {
            HttpResult<bool> result = await RestClient.PostAsync<bool>($"User/ConfirmEmailAndPassword", user, "Common");

            if (result.IsSuccessful)
            {
                return result.Content;
            }

            return null;
        }

        public async Task<bool> DeactivateUserPasswordAccount(string egnLnch)
        {
            HttpResult result = await RestClient.PostAsync<ConfirmCredentialsDto>($"User/DeactivateUserPasswordAccount", null, "Common", parameters: new { egnLnch });

            return result.IsSuccessful;

        }

        public async Task<ResponseApiDto> UpdateUserEAuthData(UserRegistrationDto user)
        {
            HttpResult result = await RestClient.PutAsync("User/UpdateUserEAuthData", user, urlExtension: "Common");

            return new ResponseApiDto
            {
                IsSuccessful = result.IsSuccessful,
                ErrorMessages = result.Error?.Type == ErrorTypeEnum.Validation ? result.Error?.Messages : null,
            };
        }

        public async Task<bool> ResentEmail(string email)
        {
            HttpResult result = await RestClient.PostAsync("User/ResendConfirmationEmail", null, urlExtension: "Common", new { email });

            return result.IsSuccessful;
        }

        public Task<bool> SendUserDeviceInfo(PublicMobileDeviceDto deviceInfo)
        {
            return RestClient.PostAsync("UserManagement/DeviceInfo", deviceInfo, alertOnException: false).IsSuccessfulResult();
        }
    }
}
