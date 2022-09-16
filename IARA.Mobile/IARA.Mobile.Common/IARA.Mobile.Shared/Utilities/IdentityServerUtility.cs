using IARA.Mobile.Application.Interfaces.Utilities;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using IdentityModel.OidcClient.Results;
using System.Net.Http;
using System.Threading.Tasks;

namespace IARA.Mobile.Shared.Utilities
{
    public class IdentityServerUtility : IIdentityServer
    {
        private readonly OidcClient _client;

        public IdentityServerUtility(IBrowser browser, IServerUrl serverUrl, IIdentityServerConfiguration configuration)
        {
            _client = new OidcClient(new OidcClientOptions
            {
                Authority = serverUrl.GetEnvironmentUrl("IARA_IDENTITY"),
                ClientId = configuration.Client,
                Scope = "openid profile offline_access",
                RedirectUri = configuration.Callback,
                Browser = browser,
                BackchannelHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true
                },
                PostLogoutRedirectUri = configuration.Callback
            });
        }
        public Task<LoginResult> Login()
        {
            return _client.LoginAsync(new LoginRequest());
        }

        public Task<LogoutResult> Logout(string token)
        {
            return _client.LogoutAsync(new LogoutRequest
            {
                IdTokenHint = token
            });
        }

        public Task<RefreshTokenResult> RefreshToken(string token)
        {
            return _client.RefreshTokenAsync(token);
        }
    }
}
