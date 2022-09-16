using System.Collections.Generic;
using IARA.Common.Exceptions;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.User;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Public
{
    // TODO add permissions and security checks!
    [AreaRoute(AreaType.Public)]
    public class MyProfilePublicController : BaseController
    {
        private readonly IMyProfileService service;
        private readonly IPersonService personService;

        public MyProfilePublicController(IMyProfileService service, IPersonService personService, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
            this.personService = personService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ProfileRead)]
        public IActionResult GetUserProfile([FromQuery] int Id)
        {
            return this.Ok(this.service.GetUserProfile(Id));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ProfileRead)]
        public IActionResult GetUserPhoto([FromQuery] int id)
        {
            return this.Ok(this.personService.GetPersonPhoto(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ProfileUpdate)]
        public IActionResult UpdateUserProfile([FromForm] MyProfileDTO userProfile)
        {
            if (this.service.HasUserEgnLnc(userProfile.EgnLnc, this.CurrentUser.ID))
            {
                this.service.UpdateUserProfile(userProfile, this.CurrentUser.ID);
                return this.Ok();
            }
            return this.NotFound();
        }

        [HttpPost]
        [CustomAuthorize]
        public IActionResult ChangeUserPassword([FromBody] UserPasswordDTO userPassword)
        {
            try
            {
                this.service.ChangeUserPassword(CurrentUser.ID, userPassword);
                return this.Ok();
            }
            catch (InvalidPasswordException ex)
            {
                List<string> messages = new List<string>();
                if (ex.Message != null && ex.Message.Length > 0)
                {
                    messages.Add(ex.Message);
                }
                else
                {
                    messages.Add(ErrorResources.msgWrongPassword);
                }
                return this.ValidationFailedResult(messages, WebHelpers.Enums.ErrorCode.WrongPassword);
            }
        }
    }
}
