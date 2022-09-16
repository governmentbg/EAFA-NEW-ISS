using System.Threading.Tasks;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IAuthenticationProvider
    {
        Task<bool> Login();
        Task Logout();
        Task SoftLogout();
        Task<bool> RefreshToken();
        bool ShouldRefreshToken();
        void Dispose(bool navigateToLogin = false);
    }
}
