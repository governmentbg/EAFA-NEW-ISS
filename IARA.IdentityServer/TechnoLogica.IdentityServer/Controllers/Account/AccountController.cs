// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TechnoLogica.Authentication.Common;
using TechnoLogica.Authentication.Common.Models;
using TechnoLogica.IdentityServer.Models;
using TechnoLogica.IdentityServer.Models.Account;
using TechnoLogica.IdentityServer.Options;
using TechnoLogica.IdentityServer.Utils;

namespace TechnoLogica.IdentityServer.Controllers.Account
{
    /// <summary>
    /// This sample controller implements a typical login/logout/provision workflow for local and external accounts.
    /// The login service encapsulates the interactions with the user data store. This data store is in-memory only and cannot be used for production!
    /// The interaction service provides a way for the UI to communicate with identityserver for validation and context retrieval
    /// </summary>
    [SecurityHeaders]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IClientStore _clientStore;
        private readonly IConfiguration _configuration;
        private readonly IEventService _events;
        private readonly IdentityServer _identityServerSettings;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly ILogger<AccountController> _logger;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly UserValidator _users;
        private readonly AuthenticationProviders authProviders;

        public AccountController(IIdentityServerInteractionService interaction,
                                 IClientStore clientStore,
                                 IAuthenticationSchemeProvider schemeProvider,
                                 IEventService events,
                                 UserValidator userValidator,
                                 ILogger<AccountController> logger,
                                 IConfiguration configuration,
                                 IdentityServer identityServerSettings,
                                 AuthenticationProviders authProviders)
        {
            this.authProviders = authProviders;
            // if the TestUserStore is not in DI, then we'll just use the global users collection
            // this is where you would plug in your own custom identity management library (e.g. ASP.NET Identity)
            _users = userValidator; //users ?? new TestUserStore(TestUsers.Users);
            _interaction = interaction;
            _clientStore = clientStore;
            _schemeProvider = schemeProvider;
            _events = events;
            _logger = logger;
            _configuration = configuration;
            _identityServerSettings = identityServerSettings;
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// Entry point into the login workflow
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            // build a model so we know what to show on the login page

            var vm = await BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {

                // we only have one option for logging in and it's an external provider
                return RedirectToAction(
                        "Challenge",
                        "External",
                        RouteValueDictionary.FromArray(new KeyValuePair<string, object>[]
                                {
                                                        new KeyValuePair<string, object>("provider", vm.ExternalLoginScheme),
                                                        new KeyValuePair<string, object>("returnUrl", returnUrl),
                                                        new KeyValuePair<string, object>("ngsw-bypass", true)
                                }));
            }

            return View(vm);
        }

        /// <summary>
        /// Handle postback from username/password login
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model, string button)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);

            // the user clicked the "cancel" button
            if (button != "login")
            {
                if (context != null)
                {
                    Client client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                    if (client != null)
                    {
                        ViewBag.ClientName = client.ClientName;
                    }

                    // if the user cancels, send a result back into IdentityServer as if they 
                    // denied the consent (even if this client does not require consent).
                    // this will send back an access denied OIDC error response to the client.
                    await _interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

                    // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                    if (await _clientStore.IsPkceClientAsync(context.Client.ClientId))
                    {
                        // if the client is PKCE then we assume it's native, so this change in how to
                        // return the response is for better UX for the end user.
                        return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });
                    }

                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    // since we don't have a valid context, then we just go back to the home page
                    return Redirect("~/");
                }
            }

            if (ModelState.IsValid)
            {
                var user = await _users.FindByUsername(IdentityServerConstants.DefaultCookieAuthenticationScheme, context?.Client.ClientId, model.Username, new List<Claim>());
                if (user != null &&
                        await _users.ValidateCredentials(IdentityServerConstants.DefaultCookieAuthenticationScheme, context?.Client.ClientId, model.Username, model.Password) &&
                        user.Active.HasValue &&
                        user.Active.Value)
                {
                    await _events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username, clientId: context?.Client.ClientId));

                    // only set explicit expiration here if user chooses "remember me". 
                    // otherwise we rely upon expiration configured in cookie middleware.
                    AuthenticationProperties props = null;
                    if (_identityServerSettings.AllowRememberLogin && model.RememberLogin)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(_identityServerSettings.RememberMeLoginDuration)
                        };
                    }
                    else if (_identityServerSettings.CookieLifetime.HasValue)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(_identityServerSettings.CookieLifetime.Value),
                            AllowRefresh = _identityServerSettings.CookieSlidingExpiration
                        };
                    }

                    // issue authentication cookie with subject ID and username
                    var clock = HttpContext.RequestServices.GetRequiredService<ISystemClock>();
                    var isUser = new IdentityServerUser(user.SubjectId)
                    {
                        DisplayName = user.Username,
                        AdditionalClaims = (new Claim[] { new Claim("ClientID", context?.Client.ClientId) }).ToArray(),
                        AuthenticationTime = clock.UtcNow.UtcDateTime
                    };
                    await HttpContext.SignInAsync(isUser, props);

                    if (context != null)
                    {
                        if (await _clientStore.IsPkceClientAsync(context.Client.ClientId))
                        {
                            // if the client is PKCE then we assume it's native, so this change in how to
                            // return the response is for better UX for the end user.
                            return View("Redirect", new RedirectViewModel { RedirectUrl = model.ReturnUrl });
                        }

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(model.ReturnUrl);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else if (string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }
                else if (
                        user != null &&
                        user.Active.HasValue &&
                        !user.Active.Value)
                {
                    await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.Client.ClientId));
                    ModelState.AddModelError(string.Empty, AccountOptions.UserNotYetActivated);
                }
                else
                {
                    await _events.RaiseAsync(new UserLoginFailureEvent(model.Username, "invalid credentials", clientId: context?.Client.ClientId));
                    ModelState.AddModelError(string.Empty, AccountOptions.InvalidCredentialsErrorMessage);
                }
            }

            // something went wrong, show form with error
            var vm = await BuildLoginViewModelAsync(model);
            return View(vm);
        }


        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {

            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(model.LogoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await HttpContext.SignOutAsync();

                // Добавено за да премахне token-ите
                await _interaction.RevokeTokensForCurrentSessionAsync();

                // Sign out from the profile for the provided clientId
                await _users.SignOutAsync(vm.ClientId);

                foreach (var provider in authProviders)
                {
                    await provider.SignOutAsync(HttpContext);
                }

                // raise the logout event
                await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });

                // Strips the leading path e.g. IdentityServer
                url = url.Substring(url.IndexOf("/Account"));
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("External logout URL: {@url}", url);
                }

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }

            return View("LoggedOut", vm);
        }

        [HttpGet]
        public IActionResult NotActive()
        {
            return View();
        }

        [HttpPost]
        [Route("api/[controller]/{client}")]
        public async Task<IActionResult> Register(string client, [FromBody] RegisterRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _users.RegisterUser(IdentityServerConstants.LocalIdentityProvider, client, model.Name, model.UserName, model.Email, model.Password, model.AdditionalAttributes);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        /*****************************************/
        /* helper APIs for the AccountController */
        /*****************************************/
        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = _identityServerSettings.AutomaticRedirectAfterSignOut,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout?.ClientName,
                ClientId = logout?.ClientId,
                SignOutIframeUrl = logout?.SignOutIFrameUrl,
                LogoutId = logoutId
            };



            if (string.IsNullOrEmpty(vm.PostLogoutRedirectUri) && _identityServerSettings.AutomaticRedirectAfterSignOut)
            {
                string clientId = logout?.ClientId ?? HttpContext?.User?.Claims?.Where(x => x.Type == "ClientID")
                        .Select(x => x.Value)
                        .FirstOrDefault();

                vm.PostLogoutRedirectUri = _identityServerSettings?.ClientPostLogoutRedirects
                        .Where(x => x.ClientId == clientId)
                        .Select(x => x.PostLogoutRedirectUrl)
                        .FirstOrDefault();
            }


            ViewBag.ClientName = vm.ClientName;

            if (User?.Identity.IsAuthenticated == true)
            {
                var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes.Where(x => x.DisplayName != null || x.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                    .Select(x => new ExternalProvider
                    {
                        DisplayName = x.DisplayName,
                        AuthenticationScheme = x.Name
                    }).ToList();

            string clientName = string.Empty;
            string registerURL = string.Empty;
            bool allowLocal = true;

            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                    clientName = client.ClientName;
                    if (client.Properties != null && client.Properties.ContainsKey("RegisterURL"))
                    {
                        registerURL = client.Properties["RegisterURL"];
                    }
                }
            }

            bool externalOnly = false;

            foreach (var provider in authProviders)
            {
                provider.BuildLogin(_configuration, this, returnUrl, providers, ref externalOnly);
            }

            ViewBag.ClientName = clientName;

            return new LoginViewModel
            {
                AllowRememberLogin = _identityServerSettings.AllowRememberLogin,
                EnableLocalLogin = allowLocal && _identityServerSettings.AllowLocalLogin && !externalOnly,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray(),
                ClientName = clientName,
                EnablePasswordReset = _identityServerSettings.EnablePasswordReset,
                RegisterURL = registerURL
            };
        }

        private async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            vm.EnablePasswordReset = _identityServerSettings.EnablePasswordReset;
            return vm;
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = _identityServerSettings.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }
    }
}
