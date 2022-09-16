using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IARA.DomainModels.DTOModels.User;
using IARA.DomainModels.DTOModels.UserManagement;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Enum;
using IARA.Security.Models;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TL.SysToSysSecCom;
using TL.SysToSysSecCom.Interfaces;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class UserManagementController : BaseAuditController
    {
        private readonly IUserManagementService service;
        private IUserService userService;
        private ICryptoHelper cryptoHelper;
        private ILogger<UserManagementController> logger;

        private const int MINUTES_TO_IMPERSONATE = 10;
        public UserManagementController(IUserManagementService service,
                                        IUserService userService,
                                        ICryptoHelper cryptoHelper,
                                        IPermissionsService permissionsService,
                                        ILogger<UserManagementController> logger)
            : base(permissionsService)
        {
            this.service = service;
            this.logger = logger;
            this.userService = userService;
            this.cryptoHelper = cryptoHelper;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ImpersonateUser)]
        public IActionResult Impersonate([FromQuery] int userId)
        {
            UserAuthDTO userAuthInfo = userService.GetUserAuthInfo(userId);
            List<int> permissionIds = permissionsService.GetUserPermissionIds(userId);

            UserSecurityModel user = new UserSecurityModel
            {
                ID = userId,
                ClientId = User.GetClientId(),
                Username = userAuthInfo.Username,
                Permissions = permissionIds.Select(x => x.ToString()).ToList(),
                LoginType = userAuthInfo.HasUserPassLogin == true ? LoginTypesEnum.PASSWORD : LoginTypesEnum.EAUTH
            };

            DateTime validTo = DateTime.Now.AddMinutes(MINUTES_TO_IMPERSONATE);

            ImpersonateSecurityModel impersonate = new ImpersonateSecurityModel
            {
                User = user,
                ValidTo = validTo
            };

            string secureJson = CommonUtils.JsonSerialize(CommonUtils.ToSecurePayload(cryptoHelper, impersonate));
            string payload = Convert.ToBase64String(Encoding.UTF8.GetBytes(secureJson));

            logger.LogWarning($"Потребител: {CurrentUser.Username} ще симулира потребител {userAuthInfo.Username} следващите {MINUTES_TO_IMPERSONATE} минути");

            return Ok(payload);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InternalUsersRead)]
        public IActionResult GetAllInternal([FromBody] GridRequestModel<UserManagementFilters> request)
        {
            IQueryable<UserDTO> result = service.GetAll(request.Filters, true);
            return PageResult(result, request, false);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ExternalUsersRead)]
        public IActionResult GetAllExternal([FromBody] GridRequestModel<UserManagementFilters> request)
        {
            IQueryable<UserDTO> result = service.GetAll(request.Filters, false);
            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ExternalUsersRead)]
        public IActionResult GetExternalUser([FromQuery] int id)
        {
            return Ok(service.GetExternalUser(id));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InternalUsersRead)]
        public IActionResult GetInternalUser([FromQuery] int id)
        {
            return Ok(service.GetInternalUser(id));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InternalUsersRead)]
        public IActionResult GetUserMobileDevices(int id)
        {
            return Ok(service.GetUserMobileDevices(id));
        }

        // TODO split into two methods for internal and external
        [HttpGet]
        [CustomAuthorize(Permissions.InternalUsersRead, Permissions.ExternalUsersRead)]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            return Ok(service.GetSimpleAudit(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InternalUsersAddRecords)]
        public IActionResult AddInternalUser([FromBody] InternalUserDTO user)
        {
            service.AddInternalUser(user);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InternalUsersAddMobileDevices)]
        public IActionResult AddOrEditMobileDevices([FromBody] List<MobileDeviceDTO> devices, [FromQuery] int userId)
        {
            service.AddOrEditInternalUserMobileDevices(userId, devices);
            return Ok();
        }

        [HttpPut]
        [CustomAuthorize(Permissions.ExternalUsersEditRecords)]
        public IActionResult EditExternalUser([FromBody] ExternalUserDTO user)
        {
            service.EditExternalUser(user);
            return Ok();
        }

        [HttpPut]
        [CustomAuthorize(Permissions.InternalUsersEditRecords)]
        public IActionResult EditInternalUser([FromBody] InternalUserDTO user)
        {
            service.EditInternalUser(user);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.InternalUsersDeleteRecords, Permissions.ExternalUsersDeleteRecords)]
        public IActionResult Delete([FromQuery] int id)
        {
            service.Delete(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.InternalUsersDeleteRecords, Permissions.ExternalUsersDeleteRecords)]
        public IActionResult UndoDelete([FromQuery] int id)
        {
            service.UndoDelete(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.InternalUsersAddRecords)]
        public IActionResult SendChangePasswordEmail([FromQuery] int userId)
        {
            service.SendChangePasswordEmail(userId);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InternalUsersAddRecords)]
        public IActionResult ChangeUserToInternal([FromBody] int userId)
        {
            service.ChangeUserToInternal(userId);
            return Ok();
        }
    }
}
