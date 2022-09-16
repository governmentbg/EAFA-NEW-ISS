using System;
using System.Collections.Generic;
using IARA.Common.Exceptions;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.User;
using IARA.Interfaces;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Public
{
    [AreaRoute(AreaType.Common)]
    public class UserController : BaseController
    {
        private readonly IUserService service;

        public UserController(IUserService service, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult GetUserAuthInfo()
        {
            UserAuthDTO userEauthInfo;

            if (CurrentUser.ID != -1)
            {
                userEauthInfo = service.GetUserAuthInfo(CurrentUser.ID);
                userEauthInfo.CurrentLoginType = CurrentUser.LoginType;
                userEauthInfo.Permissions = CurrentUser.Permissions;
            }
            else
            {
                userEauthInfo = service.GetUserAuthInfo(User.GetName());
            }

            return Ok(userEauthInfo);
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult GetUserPhoto([FromQuery] int id)
        {
            return Ok(service.GetUserPhoto(id));
        }

        [HttpPost]
        [CustomAuthorize]
        public IActionResult ConfirmEmailAndPassword([FromBody] UserLoginDTO user)
        {
            return Ok(this.service.ConfirmUserEmailAndPassword(CurrentUser.ID, user));
        }

        [HttpPut]
        [CustomAuthorize]
        public IActionResult UpdateUserEAuthData([FromBody] UserRegistrationDTO user)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    this.service.UpdateUserEAuthData(user);

                    return Ok();
                }
                catch (EmailExistsException ex)
                {
                    List<string> messages = new List<string>();
                    if (ex.Message != null && ex.Message.Length > 0)
                    {
                        messages.Add(ex.Message);
                    }
                    else
                    {
                        messages.Add(ErrorResources.msgEmailExists);
                    }

                    return base.ValidationFailedResult(messages, WebHelpers.Enums.ErrorCode.InvalidEmail);
                }
            }

            return base.ValidationFailedResult();
        }

        [HttpPost]
        public IActionResult AddExternalUser([FromBody] UserRegistrationDTO user)
        {
            if (this.ModelState.IsValid)
            {
                try
                {
                    int result = this.service.AddExternalUser(user);

                    return Ok(result);
                }
                catch (EgnLnchExistsException ex)
                {
                    List<string> messages = new List<string>();
                    if (ex.Message != null && ex.Message.Length > 0)
                    {
                        messages.Add(ex.Message);
                    }
                    else
                    {
                        messages.Add(ErrorResources.msgEgnLnchExists);
                    }

                    return base.ValidationFailedResult(messages, WebHelpers.Enums.ErrorCode.InvalidEgnLnch);
                }
                catch (EmailExistsException ex)
                {
                    List<string> messages = new List<string>();
                    if (ex.Message != null && ex.Message.Length > 0)
                    {
                        messages.Add(ex.Message);
                    }
                    else
                    {
                        messages.Add(ErrorResources.msgEmailExists);
                    }

                    return base.ValidationFailedResult(messages, WebHelpers.Enums.ErrorCode.InvalidEmail);
                }
            }
            return base.ValidationFailedResult();
        }

        [HttpPost]
        public IActionResult UpdateUserRegistration([FromBody] UserRegistrationDTO user)
        {
            return Ok(this.service.UpdateUserRegistrationInfo(user));
        }

        [HttpPost]
        [CustomAuthorize]
        public IActionResult DeactivateUserPasswordAccount([FromQuery] string egnLnch)
        {
            this.service.DeactivateUserPasswordAccount(egnLnch);
            return Ok();
        }

        [HttpPost]
        public IActionResult ResendConfirmationEmail([FromQuery] string email)
        {
            this.service.SendConfirmationEmail(email);
            return Ok();
        }

        [HttpPost]
        public IActionResult ResendConfirmationEmailForToken([FromBody] UserTokenDTO userTokenInfo)
        {
            string email = this.service.GetUserEmailByConfirmationKey(userTokenInfo.Token);
            this.service.SendConfirmationEmail(email);

            return Ok();
        }

        [HttpGet]
        public IActionResult EmailConfirmation([FromQuery] string token)
        {
            bool isConfirmed = this.service.ConfirmEmailToken(token);

            return isConfirmed ? LocalRedirect("/") : NotFound();
        }

        [HttpPost]
        public IActionResult ChangePassword([FromBody] UserChangePasswordDTO data)
        {
            this.service.ChangeUserPassword(data);
            return Ok();
        }

        [HttpPost]
        public IActionResult ActivateUserAccount([FromBody] UserTokenDTO token)
        {
            return Ok(this.service.ActivateUserAccount(token));
        }

        [HttpPost]
        [CustomAuthorize]
        public IActionResult UpdateAllUserData([FromBody] ChangeUserDataDTO userData)
        {
            userData.Id = CurrentUser.ID;
            bool result = service.UpdateUserInfo(userData);
            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult GetAllUserData()
        {
            var userData = service.GetAllUserData(CurrentUser.ID);
            return Ok(userData);
        }
    }
}
