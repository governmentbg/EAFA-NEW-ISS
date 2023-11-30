using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TechnoLogica.IdentityServer.Models.Account;
using TechnoLogica.IdentityServer.Utils;

namespace TechnoLogica.IdentityServer.Controllers.Account
{
    [SecurityHeaders]
    [Authorize]
    public class PasswordController : Controller
    {
        UserValidator _userValidator;
        IClientStore _clientStore;
        private readonly IIdentityServerInteractionService _interaction;
        private ILogger<PasswordController> _logger { get; set; }

        public PasswordController(IClientStore clientStore,
                                  UserValidator userValidator,
                                  IIdentityServerInteractionService interaction,
                                  ILogger<PasswordController> logger)
        {
            _clientStore = clientStore;
            _userValidator = userValidator;
            _interaction = interaction;
            _logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetToken(string clientId, string resetToken, string eMail)
        {
            var client = await _clientStore.FindEnabledClientByIdAsync(clientId);

            var vm = new ResetTokenPasswordModel()
            {
                ClientName = client.ClientName,
                ClientId = clientId,
                ResetToken = resetToken
            };

            ViewBag.ClientName = vm.ClientName;

            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetToken(ResetTokenPasswordModel model, string button)
        {
            switch (button)
            {
                case "reset":
                    {
                        if (model.Password != model.NewPassword)
                        {
                            ModelState.AddModelError(string.Empty, $"Грешка при възстановяване на парола: Парола и Нова Парола трябва да съвпадат");
                            return View(new ResetTokenPasswordModel() { ClientName = model.ClientName, EMail = model.EMail, ResetToken = model.ResetToken });
                        }
                        else
                        {
                            try
                            {
                                var result = await _userValidator.ResetPassword(
                                    IdentityServerConstants.DefaultCookieAuthenticationScheme,
                                    model.ClientId,
                                    model.EMail,
                                    model.ResetToken,
                                    model.Password);
                                if (result.Succeeded)
                                {
                                    var client = await _clientStore.FindEnabledClientByIdAsync(model.ClientId);
                                    var redirectURI = client.PostLogoutRedirectUris.FirstOrDefault();
                                    return View(new ResetTokenPasswordModel() { ClientName = model.ClientName, Result = true, RedirectURI = redirectURI });
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, "Неуспешно възстановяване на парола");
                                    return View(new ResetTokenPasswordModel() { ClientName = model.ClientName, EMail = model.EMail, ResetToken = model.ResetToken });
                                }
                            }
                            catch (Exception ex)
                            {
                                ModelState.AddModelError(string.Empty, $"Грешка при възстановяване на парола: {ex.Message}");
                                _logger.LogError(ex, "Грешка при възстановяване на парола");
                                return View(new ResetTokenPasswordModel() { ClientName = model.ClientName, EMail = model.EMail, ResetToken = model.ResetToken });
                            }
                        }
                    }
                default:
                    {
                        return Redirect("~/");
                    }
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Reset(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            var client = context?.Client != null ? await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId) : null;

            var vm = new ResetPasswordModel()
            {
                ReturnUrl = returnUrl,
                ClientName = client?.ClientName
            };

            ViewBag.ClientName = vm.ClientName;
            return View(vm);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Reset(ResetPasswordModel model, string button)
        {
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            switch (button)
            {
                case "reset":
                    {
                        try
                        {
                            var result = await _userValidator.SendPasswordResetToken(
                                IdentityServerConstants.DefaultCookieAuthenticationScheme,
                                context.Client.ClientId,
                                $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}{HttpContext.Request.PathBase}/Password/ResetToken",
                                model.EMail);
                            if (result)
                            {
                                return View(new ResetPasswordModel() { ReturnUrl = model.ReturnUrl, ClientName = model.ClientName, Result = true });
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Неуспешно възстановяване на парола");
                                return View(new ResetPasswordModel() { ReturnUrl = model.ReturnUrl, ClientName = model.ClientName });
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(string.Empty, $"Грешка при възстановяване на парола: {ex.Message}");
                            return View(new ResetPasswordModel() { ReturnUrl = model.ReturnUrl, ClientName = model.ClientName });
                        }
                    }
                default:
                    {
                        return Redirect("~/");
                    }
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Change(string returnUrl)
        {
            // build a model so we know what to show on the login page

            var clientID = HttpContext.User.Claims.Where(c => c.Type == "ClientID").Select(c => c.Value).FirstOrDefault();
            var client = await _clientStore.FindEnabledClientByIdAsync(clientID);
            var vm = new ChangePasswordModel() { ReturnUrl = returnUrl, ClientName = client.ClientName };
            ViewBag.ClientName = vm.ClientName;

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Change(ChangePasswordModel model, string button)
        {
            ViewBag.ClientName = model.ClientName;
            switch (button)
            {
                case "change":
                    {
                        var clientID = HttpContext.User.Claims.Where(c => c.Type == "ClientID").Select(c => c.Value).FirstOrDefault();
                        var username = HttpContext.User.Claims.Where(c => c.Type == "name").Select(c => c.Value).FirstOrDefault();

                        try
                        {
                            var result = await _userValidator.ChangePassword(
                                IdentityServerConstants.DefaultCookieAuthenticationScheme,
                                clientID,
                                username,
                                model.OldPassword,
                                model.NewPassword);
                            if (result)
                            {
                                return Redirect(model.ReturnUrl);
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "Неуспешна промяна на парола");
                                return View(new ChangePasswordModel() { ReturnUrl = model.ReturnUrl, ClientName = model.ClientName });
                            }
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError(string.Empty, $"Грешка при промяна на парола: {ex.Message}");
                            return View(new ChangePasswordModel() { ReturnUrl = model.ReturnUrl, ClientName = model.ClientName });
                        }
                    }
                default:
                    {
                        return Redirect(model.ReturnUrl);
                    }
            }
        }
    }
}
