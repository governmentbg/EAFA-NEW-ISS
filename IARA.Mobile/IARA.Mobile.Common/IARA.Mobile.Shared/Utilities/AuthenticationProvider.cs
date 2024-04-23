using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Domain.Models;
using IARA.Mobile.Shared.Helpers;
using IARA.Mobile.Shared.ResourceTranslator;
using IdentityModel.OidcClient;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

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

        public bool CheckTokenValidity()
        {
            if (string.IsNullOrEmpty(_authTokenProvider.Token))
            {
                return false;
            }
            else if (_authTokenProvider.AccessTokenExpiration < DateTime.Now)
            {
                return false;
            }

            return true;
        }

        public void SetAuthenticationProvider(JwtToken token)
        {
            _authTokenProvider.Clear();
            _authTokenProvider.Token = token.Token;
            _authTokenProvider.RefreshToken = token.RefreshToken;
            _authTokenProvider.AccessTokenExpiration = token.ValidTo;
        }

        public async Task Logout()
        {
            //await _identityServer.Logout(_authTokenProvider.Token);
            await Task.Delay(400);
            Dispose();
        }

        public async Task SoftLogout()
        {
            //await _identityServer.Logout(_authTokenProvider.Token);
            _authTokenProvider.Clear();
            await Task.Delay(400);
        }

        public async Task<bool> RefreshToken()
        {
            HttpResult<JwtToken> result = await DependencyService.Resolve<IRestClient>().PostAsync<JwtToken>("Security/RefreshToken", "Common", false, new JwtToken()
            {
                Token = _authTokenProvider.RefreshToken
            });

            if (result.IsSuccessful)
            {
                _authTokenProvider.Token = result.Content.Token;
                _authTokenProvider.AccessTokenExpiration = result.Content.ValidTo;

                return true;
            }

            return false;
        }

        public bool ShouldRefreshToken()
        {
            DateTime accessTokenExpiration = _authTokenProvider.AccessTokenExpiration;
            DateTime now = DateTime.UtcNow;
            Debug.Write($"Access Token Expiration: {accessTokenExpiration:MM/dd/yyyy HH:mm:ss}, Current UTC:{now}");
            return accessTokenExpiration != DateTime.MinValue &&
                        accessTokenExpiration.AddSeconds(-30) < now &&
                            !string.IsNullOrEmpty(_authTokenProvider.RefreshToken);
        }

        public void Dispose(bool navigateToLogin = false)
        {
            _translator.ClearResources();
            _authTokenProvider.Clear();
            Translator.Current.LoadOfflineResources();

            if (navigateToLogin)
            {
                _commonNavigator.ToLogin();
            }
        }
    }
}
