using System.Security.Authentication;
using IARA.DomainModels.DTOModels.User;
using IARA.Interfaces;
using IARA.WebHelpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TL.AspNet.Security.Abstractions.Enums;
using TL.AspNet.Security.Abstractions.Models;
using TL.AspNet.Security.Abstractions.Services;
using TL.AspNet.Security.eAuth.Configuration;
using TL.AspNet.Security.eAuth.Controllers;
using TL.AspNet.Security.eAuth.eGov.Extensions;
using TL.AspNet.Security.eAuth.eGov.Identity;
using TL.AspNet.Security.eAuth.Requests;
using TL.Logging.Abstractions.Interfaces;

namespace IARA.WebAPI.Controllers.Common
{
    [AreaRoute(AreaType.Common)]
    public class SecurityController : EAuthBaseSecurityController<int>
    {
        private readonly IUserService userService;
        private IExtendedLogger logger;

        public SecurityController(IUserService userService,
                                  IExtendedLogger logger,
                                  IEGovUserManager<int> userManager)
            : base(userManager)
        {
            this.logger = logger;
            this.userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult SignIn([FromBody] AuthCredentials credentials)
        {
            IActionResult result = Login(credentials, type: LoginTypes.UsernamePassword);

            return result;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetUser()
        {
            int userId = this.GetUserIdentifier();
            UserAuthDTO user = userService.GetUserAuthInfo(userId);

            return Ok(user);
        }

        protected override LoginResultModel CredentialsLogin<TCredentials>(IUserCredentialsManager<int, TCredentials> userManager,
                                                                           TCredentials credentials,
                                                                           LoginTypes loginType,
                                                                           out AuthenticationResult result,
                                                                           bool issueTempToken = false)
        {
            LoginResultModel loginResult = base.CredentialsLogin(userManager, credentials, loginType, out result, issueTempToken);

            if (loginResult.Type != LoginResultTypes.Success)
            {
                //SendFailedLoginNotification((credentials as Credentials).UserName);
            }

            return loginResult;
        }

        protected override LoginResultModel CreateCredentialsForSamlAuth(EGovIdentity eGovIdentity, out AuthenticationResult result, out int userIdentifier)
        {
            LoginResultModel loginResult;

            userIdentifier = userManager.GetIdentifierByEGN(eGovIdentity.PersonIdentifier);

            if (userIdentifier != 0)
            {
                loginResult = userManager.ValidateCredentials(eGovIdentity.PersonIdentifier, this.GetUserContext(this.HttpContext, LoginTypes.EAuth), out result);
            }
            else
            {
                result = null;
                loginResult = new LoginResultModel(LoginResultTypes.UserMissingInDB);
            }

            if (loginResult.Type != LoginResultTypes.Success)
            {
                //SendFailedLoginNotification(eGovIdentity.PersonIdentifier);
            }

            return loginResult;
        }

        protected override EGovIdentity ReadSamlResponse(Saml2Configuration saml2Configuration, out Saml2AuthnResponse samlResponse)
        {
            samlResponse = null;

            try
            {
                return base.ReadSamlResponse(saml2Configuration, out samlResponse);
            }
            catch (AuthenticationException)
            {
                if (samlResponse != null)
                {
                    string userIdentifier = samlResponse.ClaimsIdentity?.GetPersonIdentifier();

                    if (!string.IsNullOrEmpty(userIdentifier))
                    {
                        //SendFailedLoginNotification(userIdentifier);
                    }
                }

                throw;
            }
        }
    }
}
