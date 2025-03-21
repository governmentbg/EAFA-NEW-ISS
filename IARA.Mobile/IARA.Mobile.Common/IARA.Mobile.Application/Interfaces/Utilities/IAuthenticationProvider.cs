using IARA.Mobile.Pub.Domain.Models;
using System.Threading.Tasks;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IAuthenticationProvider
    {
        Task Logout();
        Task SoftLogout();
        Task<bool> RefreshToken();
        bool ShouldRefreshToken();
        void Dispose(bool navigateToLogin = false);
        void SetAuthenticationProvider(JwtToken token);
        bool CheckTokenValidity();
    }
}
