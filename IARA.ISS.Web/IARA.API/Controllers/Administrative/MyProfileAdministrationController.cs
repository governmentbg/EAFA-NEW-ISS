using System.Collections.Generic;
using IARA.Common.Exceptions;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.User;
using IARA.Interfaces;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Administrative
{
    // TODO add permissions
    [AreaRoute(AreaType.Administrative)]
    public class MyProfileAdministrationController : BaseAuditController
    {
        private readonly IMyProfileService service;
        private readonly IPersonService personService;

        public MyProfileAdministrationController(IPermissionsService permissionsService,
                                                 IMyProfileService service,
                                                 IPersonService personService)
            : base(permissionsService)
        {
            this.service = service;
            this.personService = personService;
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult GetUserProfile([FromQuery] int Id)
        {
            return Ok(service.GetUserProfile(Id));
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult GetUserPhoto([FromQuery] int id)
        {
            return Ok(personService.GetPersonPhoto(id));
        }

        [HttpPost]
        [CustomAuthorize]
        public IActionResult UpdateUserProfile([FromForm] MyProfileDTO userProfile)
        {
            service.UpdateUserProfile(userProfile, CurrentUser.ID);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            return Ok(service.GetSimpleAudit(id));
        }

        [HttpPost]
        [CustomAuthorize]
        public IActionResult ChangeUserPassword([FromBody] UserPasswordDTO userPassword)
        {
            try
            {
                service.ChangeUserPassword(CurrentUser.ID, userPassword);
                return Ok();
            }
            catch (InvalidPasswordException ex)
            {
                List<string> messages = new();
                if (ex.Message != null && ex.Message.Length > 0)
                {
                    messages.Add(ex.Message);
                }
                else
                {
                    messages.Add(ErrorResources.msgWrongPassword);
                }
                return ValidationFailedResult(messages, WebHelpers.Enums.ErrorCode.WrongPassword);
            }
        }
    }
}
