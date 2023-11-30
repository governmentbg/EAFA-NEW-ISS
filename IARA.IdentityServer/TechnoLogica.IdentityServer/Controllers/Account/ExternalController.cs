using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TechnoLogica.Authentication.Common;
using TechnoLogica.Authentication.Common.Models;
using TechnoLogica.IdentityServer.Models.Account;
using TechnoLogica.IdentityServer.Models.External;
using TechnoLogica.IdentityServer.Utils;

namespace TechnoLogica.IdentityServer.Controllers.Account
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class ExternalController : Controller
    {
        private readonly UserValidator _userValidator;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly ILogger<ExternalController> _logger;
        private readonly IEventService _events;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IConfiguration _configuration;
        private readonly AuthenticationProviders authProviders;

        public ExternalController(IIdentityServerInteractionService interaction,
                                  IClientStore clientStore,
                                  IEventService events,
                                  ILogger<ExternalController> logger,
                                  IAuthenticationSchemeProvider schemeProvider,
                                  UserValidator userValidator,
                                  IConfiguration configuration,
                                  AuthenticationProviders authProviders)
        {
            this.authProviders = authProviders;
            this._interaction = interaction;
            this._clientStore = clientStore;
            this._logger = logger;
            this._events = events;
            this._schemeProvider = schemeProvider;
            this._userValidator = userValidator;
            this._configuration = configuration;
        }

        [HttpPost]
        public IActionResult ChallengePost(string provider, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";
            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)),
                Items =
                                        {
                                                { "returnUrl", returnUrl },
                                                { "scheme", provider },
                                        }
            };

            return Challenge(props, provider);
        }

        /// <summary>
        /// initiate roundtrip to external authentication provider
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Challenge(string provider, string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl)) returnUrl = "~/";

            // validate returnUrl - either it is a valid OIDC URL or back to a local page
            if (Url.IsLocalUrl(returnUrl) == false && _interaction.IsValidReturnUrl(returnUrl) == false)
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            string clientName = string.Empty;
            if (context != null)
            {
                if (context?.Client.ClientId != null)
                {
                    var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                    clientName = client?.ClientName;
                }
            }

            foreach (var exportedProvider in authProviders)
            {
                var result = await exportedProvider.Challenge(this._configuration, this, provider, returnUrl, clientName);

                if (result != null)
                {
                    return result;
                }
            }

            // start challenge and roundtrip the return URL and scheme 
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(Callback)),
                Items =
                                        {
                                                { "returnUrl", returnUrl },
                                                { "scheme", provider },
                                        }
            };

            return Challenge(props, provider);
        }

        /// <summary>
        /// Post processing of external authentication
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Callback()
        {
            // read external identity from the temporary cookie
            var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            var additionalClaims = new Dictionary<string, string>();

            foreach (var p in authProviders)
            {
                result = await p.ExternalCallback(result, HttpContext, additionalClaims);
            }
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                var externalClaims = result.Principal.Claims.Select(c => $"{c.Type}: {c.Value}");
                _logger.LogDebug("External claims: {@claims}", externalClaims);
            }

            // lookup our user and external provider info
            var (hasContext, user, clientId, userId, returnUrl, provider) = await FindUserFromExternalProvider(result);
            if (hasContext)
            {
                if (user == null || string.IsNullOrEmpty(user.SubjectId))
                {
                    var registrationResult = await AutoProvisionUser(
                            result.Principal.Identity.AuthenticationType,
                            result,
                            clientId,
                            userId,
                            additionalClaims);

                    if (registrationResult.Succeeded)
                    {
                        user = await _userValidator.FindByUsername(provider, clientId, userId, result.Principal.Claims.ToList());
                        if (user != null && user.Active.HasValue && user.Active.Value)
                        {
                            return await RedirectToReturnURL(result, user, userId, returnUrl, provider);
                        }
                        else
                        {
                            return await LogOutAndRedirectToNotActive();
                        }
                    }
                    else
                    {
                        return await LogOutAndRedirectToErrorReigstration(registrationResult.Errors, clientId);
                    }
                }
                else if (user.Active.HasValue && !user.Active.Value)
                {
                    return await LogOutAndRedirectToNotActive();
                }
                else
                {
                    return await RedirectToReturnURL(result, user, userId, returnUrl, provider);
                }
            }
            else
            {
                return Redirect(returnUrl);
            }
        }

        private async Task<IActionResult> RedirectToReturnURL(AuthenticateResult result, UserInfo user, string userId, string returnUrl, string provider)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            // this allows us to collect any additonal claims or properties
            // for the specific prtotocols used and store them in the local auth cookie.
            // this is typically used to store data needed for signout from those protocols.
            var additionalLocalClaims = new List<Claim>();
            var localSignInProps = new AuthenticationProperties();
            additionalLocalClaims.Add(new Claim("ClientID", context?.Client.ClientId));
            ProcessLoginCallbackForOidc(result, additionalLocalClaims, localSignInProps);

            // issue authentication cookie for user
            var clock = HttpContext.RequestServices.GetRequiredService<ISystemClock>();
            var isUser = new IdentityServerUser(user.SubjectId)
            {
                DisplayName = user.Username,
                IdentityProvider = provider,
                AdditionalClaims = additionalLocalClaims.ToArray(),
                AuthenticationTime = clock.UtcNow.UtcDateTime
            };

            await HttpContext.SignInAsync(isUser, localSignInProps);

            // delete temporary cookie used during external authentication
            await HttpContext.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            // check if external login is in the context of an OIDC request
            await _events.RaiseAsync(new UserLoginSuccessEvent(provider, userId, user.SubjectId, user.Username, true, context?.Client.ClientId));

            if (context != null)
            {
                if (await _clientStore.IsPkceClientAsync(context.Client.ClientId))
                {
                    // if the client is PKCE then we assume it's native, so this change in how to
                    // return the response is for better UX for the end user.
                    return View("Redirect", new RedirectViewModel { RedirectUrl = returnUrl });
                }
            }

            return Redirect(returnUrl);
        }
        private async Task<IActionResult> LogOutAndRedirectToErrorReigstration(IEnumerable<UserRegistrationError> errors, string clientId)
        {
            // delete local authentication cookie
            await HttpContext.SignOutAsync();
            // Добавено за да премахне token-ите
            await _interaction.RevokeTokensForCurrentSessionAsync();

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"Cannot register");
            }

            string postLogoutRedirectUri = null;
            var client = await _clientStore.FindEnabledClientByIdAsync(clientId);

            if (client != null)
            {
                postLogoutRedirectUri = client.PostLogoutRedirectUris.FirstOrDefault();
            }

            return View("ErrorRegistration", new RegistrationErrorModel()
            {
                Errors = errors,
                PostLogoutRedirectUri = postLogoutRedirectUri
            });
        }

        private async Task<IActionResult> LogOutAndRedirectToNotActive()
        {
            // delete local authentication cookie
            await HttpContext.SignOutAsync();
            // Добавено за да премахне token-ите
            await _interaction.RevokeTokensForCurrentSessionAsync();

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"User not active. Redirecting to ~/Account/NotActive");
            }

            return RedirectToAction("NotActive", "Account");
        }

        private async Task<(bool hasContext,
                UserInfo user,
                string clientId,
                string providerUserId,
                string returnUrl,
                string provider)> FindUserFromExternalProvider(AuthenticateResult result)
        {
            var externalUser = result.Principal;

            // try to determine the unique id of the external user (issued by the provider)
            // the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = externalUser.FindFirst(JwtClaimTypes.Subject) ??
                                                externalUser.FindFirst(ClaimTypes.NameIdentifier) ??
                                                throw new Exception("Unknown userid");

            // remove the user id claim so we don't include it as an extra claim if/when we provision the user
            var claims = externalUser.Claims.ToList();
            claims.Remove(userIdClaim);

            var provider = result.Properties.Items["scheme"];
            var providerUserId = userIdClaim.Value;

            // find external user
            // retrieve return URL
            var returnUrl = result.Properties.Items["returnUrl"] ?? "~/";

            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            UserInfo user = null;
            bool hasClientContext = !string.IsNullOrEmpty(context?.Client.ClientId);

            if (hasClientContext)
            {
                user = await _userValidator.FindByUsername(provider, context.Client.ClientId, providerUserId, claims);
            }

            return (hasClientContext, user, context?.Client.ClientId, providerUserId, returnUrl, provider);
        }

        private async Task<UserRegistrationResult> AutoProvisionUser(string scheme,
                                                                     AuthenticateResult result,
                                                                     string clientId,
                                                                     string userId,
                                                                     Dictionary<string, string> additionalClaims)
        {
            additionalClaims.TryGetValue(IAuthenticationProvider.ADDITIONAL_CLAIM_NAME, out string name);
            additionalClaims.TryGetValue(IAuthenticationProvider.ADDITIONAL_CLAIM_EMAIL, out string email);

            return await _userValidator.RegisterUser(scheme, clientId, name, userId, email, new Guid().ToString() + userId, additionalClaims);
        }

        private void ProcessLoginCallbackForOidc(AuthenticateResult externalResult, List<Claim> localClaims, AuthenticationProperties localSignInProps)
        {
            // if the external system sent a session id claim, copy it over
            // so we can use it for single sign-out
            var sid = externalResult.Principal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId);
            if (sid != null)
            {
                localClaims.Add(new Claim(JwtClaimTypes.SessionId, sid.Value));
            }

            // if the external provider issued an id_token, we'll keep it for signout
            var id_token = externalResult.Properties.GetTokenValue("id_token");
            if (id_token != null)
            {
                localSignInProps.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = id_token } });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Error([FromQuery(Name = ".redirect")] string redirect,
                                               [FromQuery(Name = "returnUrl")] string returnURL,
                                               [FromQuery(Name = "scheme")] string scheme,
                                               [FromQuery(Name = "error_messages")] string messages,
                                               [FromQuery(Name = "error_hidden_messages")] string hiddenMessages)
        {
            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var schemeDisplayName =
                    schemes
                    .Where(s => s.Name == scheme)
                    .Select(s => s.DisplayName)
                    .FirstOrDefault();

            var vm = new ExternalLoginErrorViewModel()
            {
                SchemeDisplayName = schemeDisplayName,
                Messages = messages?.Split(Environment.NewLine).ToList(),
                HiddenMessages = hiddenMessages?.Split(Environment.NewLine).ToList(),
            };

            return View(vm);
        }
    }
}
