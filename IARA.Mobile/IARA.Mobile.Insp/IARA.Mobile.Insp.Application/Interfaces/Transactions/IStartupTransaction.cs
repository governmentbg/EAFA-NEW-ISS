using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Application.Interfaces.Utilities;
using System;
using System.Threading.Tasks;

namespace IARA.Mobile.Insp.Application.Interfaces.Transactions
{
    public interface IStartupTransaction
    {
        Task<bool> HealthCheck();

        Task<bool> IsAppOutdated(int version, string platform);

        Task<bool?> IsDeviceAllowed(string imei, IAuthTokenProvider tokenProvider);

        Task<UserAuthDto> GetUserAuthInfo();

        Task SendUserDeviceInfo(PublicMobileDeviceDto info);

        Task PostOfflineData();

        Task<bool> GetInitialData(bool isLoginRequest, Action<int> countCallback, Action finishCallback);
    }
}
