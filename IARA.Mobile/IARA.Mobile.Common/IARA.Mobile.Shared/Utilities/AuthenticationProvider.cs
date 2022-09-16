using IARA.Mobile.Application.Interfaces.Utilities;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Results;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace IARA.Mobile.Shared.Utilities
{
    public class AuthenticationProvider : IAuthenticationProvider
    {
        public const string LoginType = nameof(LoginType);

        private readonly IIdentityServer _identityServer;
        private readonly IAuthTokenProvider _authTokenProvider;
        private readonly ITranslator _translator;
        private readonly ICommonNavigator _commonNavigator;

        public AuthenticationProvider(IIdentityServer identityServer, IAuthTokenProvider authTokenProvider, ITranslator translator, ICommonNavigator commonNavigator)
        {
            _identityServer = identityServer ?? throw new ArgumentNullException(nameof(identityServer));
            _authTokenProvider = authTokenProvider ?? throw new ArgumentNullException(nameof(authTokenProvider));
            _translator = translator ?? throw new ArgumentNullException(nameof(translator));
            _commonNavigator = commonNavigator ?? throw new ArgumentNullException(nameof(commonNavigator));
        }

        public async Task<bool> Login()
        {
            LoginResult result = await _identityServer.Login();
            bool successLogin = !string.IsNullOrEmpty(result?.AccessToken);
            if (successLogin)
            {
                Debug.WriteLine($"Access TOKEN: {result.AccessToken}");
                _authTokenProvider.Clear();
                _authTokenProvider.Token = result.AccessToken;
                _authTokenProvider.RefreshToken = result.RefreshToken;
                _authTokenProvider.AccessTokenExpiration = result.AccessTokenExpiration.UtcDateTime;
            }

            return successLogin;
        }

        public async Task Logout()
        {
            await _identityServer.Logout(_authTokenProvider.Token);
            await Task.Delay(400);
            Dispose();
        }

        public async Task SoftLogout()
        {
            await _identityServer.Logout(_authTokenProvider.Token);
            _authTokenProvider.Clear();
            await Task.Delay(400);
        }

        public async Task<bool> RefreshToken()
        {
            RefreshTokenResult result = await _identityServer.RefreshToken(_authTokenProvider.RefreshToken);
            bool successLogin = !string.IsNullOrEmpty(result?.AccessToken);
            Debug.WriteLine($"Refreshed success: {successLogin}");
            if (successLogin)
            {
                Debug.WriteLine($"Refreshed Access TOKEN: {result.AccessToken}");
                _authTokenProvider.Clear();
                _authTokenProvider.Token = result.AccessToken;
                _authTokenProvider.RefreshToken = result.RefreshToken;
                _authTokenProvider.AccessTokenExpiration = result.AccessTokenExpiration.UtcDateTime;
            }
            return successLogin;
        }

        public bool ShouldRefreshToken()
        {
            DateTime accessTokenExpiration = _authTokenProvider.AccessTokenExpiration;
            DateTime now = DateTime.UtcNow;
            Debug.Write($"Access Token Expiration: {accessTokenExpiration.ToString("MM/dd/yyyy HH:mm:ss")}, Current UTC:{now}");
            return accessTokenExpiration != DateTime.MinValue &&
                        accessTokenExpiration.AddSeconds(-30) < now &&
                            !string.IsNullOrEmpty(_authTokenProvider.RefreshToken);
        }

        public void Dispose(bool navigateToLogin = false)
        {
            _translator.ClearResources();
            _translator.LoadOfflineResources();
            _authTokenProvider.Clear();
            if (navigateToLogin)
                _commonNavigator.ToLogin();

        }
    }
}
