using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Pub.Application.DTObjects.User.API;
using System.Threading.Tasks;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface IUserTransaction
    {
        Task<ResponseApiDto> AddUser(UserRegistrationDto user);
        Task<bool?> ConfirmEmailAndPassword(ConfirmUserDataDto user);
        Task<bool> DeactivateUserPasswordAccount(string egn);
        Task<ResponseApiDto> UpdateUserEAuthData(UserRegistrationDto user);
        Task<bool> ResentEmail(string email);
        Task<bool> SendUserDeviceInfo(PublicMobileDeviceDto deviceInfo);
    }
}
