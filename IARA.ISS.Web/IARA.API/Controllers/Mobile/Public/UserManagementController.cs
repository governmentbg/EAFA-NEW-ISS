using System;
using IARA.DomainModels.DTOModels.UserManagement;
using IARA.Interfaces;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Mobile.Public
{
    [AreaRoute(AreaType.MobilePublic)]
    public class UserManagementController : BaseController
    {
        private readonly IUserManagementService userManagementService;

        public UserManagementController(IUserManagementService userManagementService, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.userManagementService = userManagementService ?? throw new ArgumentNullException(nameof(userManagementService));
        }

        [HttpPost]
        [CustomAuthorize]
        public IActionResult DeviceInfo(PublicMobileDeviceDTO device)
        {
            this.userManagementService.AddOrEditPublicUserDevice(this.CurrentUser.ID, false, device);
            return this.Ok();
        }
    }
}
