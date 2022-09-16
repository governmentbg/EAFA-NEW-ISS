using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Results;
using System.Threading.Tasks;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IIdentityServer
    {
        Task<LoginResult> Login();
        Task<LogoutResult> Logout(string token);
        Task<RefreshTokenResult> RefreshToken(string token);
    }
}
