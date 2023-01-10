using System.Threading.Tasks;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Results;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IIdentityServer
    {
        Task<LoginResult> Login();
        Task<LogoutResult> Logout(string token);
        Task<RefreshTokenResult> RefreshToken(string token);
    }
}
