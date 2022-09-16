using IARA.Mobile.Application.DTObjects.Users;
using System;
using System.Threading.Tasks;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface IStartupTransaction
    {
        Task<bool> HealthCheck();

        Task<bool> IsAppOutdated(int version, string platform);

        Task<UserAuthDto> GetUserAuthInfo();

        Task PostOfflineData();

        Task<bool> GetInitialData(bool isLoginRequest, Action<int> countCallback, Action finishCallback);
    }
}
