using System;
using System.Collections.Generic;
using IARA.Common.Exceptions;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.User;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Mobile.Public
{
    [AreaRoute(AreaType.MobilePublic)]
    public class ProfileController : BaseController
    {
        private readonly IMyProfileService _profile;
        private readonly IPersonService _personService;

        public ProfileController(IMyProfileService profile, IPersonService personService, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this._profile = profile ?? throw new ArgumentNullException(nameof(profile));
            this._personService = personService ?? throw new ArgumentNullException(nameof(personService));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ProfileRead)]
        public IActionResult Get()
        {
            return this.Ok(this._profile.GetUserProfile(this.CurrentUser.ID));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ProfileRead)]
        public IActionResult GetUserPhoto()
        {
            DownloadableFileDTO photo = this._personService.GetPersonPhotoAsModel(this.CurrentUser.ID);

            if (photo == null)
            {
                return this.Ok();
            }

            return this.File(photo.Bytes, photo.MimeType, photo.FileName);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ProfileUpdate)]
        public IActionResult Edit([FromForm] MyProfileDTO profile)
        {
            if (this._profile.HasUserEgnLnc(profile.EgnLnc, this.CurrentUser.ID))
            {
                this._profile.UpdateUserProfile(profile, this.CurrentUser.ID);
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
                this._profile.ChangeUserPassword(this.CurrentUser.ID, userPassword);
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
