using System;
using IARA.DomainModels.DTOModels.UserManagement;
using IARA.Interfaces;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Mobile.Administrative
{
    [AreaRoute(AreaType.MobileAdministrative)]
    public class UserManagementController : BaseController
    {
        private readonly IUserManagementService service;

        public UserManagementController(IUserManagementService service, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost]
        [CustomAuthorize]
        public IActionResult DeviceInfo(PublicMobileDeviceDTO device)
        {
            this.service.AddOrEditPublicUserDevice(this.CurrentUser.ID, true, device);
            return this.Ok();
        }
    }
}
