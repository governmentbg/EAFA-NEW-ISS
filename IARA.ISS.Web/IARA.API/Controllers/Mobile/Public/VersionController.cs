using IARA.Common.Enums;
using IARA.Interfaces;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Mobile.Public
{
    [AreaRoute(AreaType.MobilePublic)]
    [AllowAnonymous]
    public class VersionController : BaseController
    {
        private readonly IMobileVersionService versionService;

        public VersionController(IPermissionsService permissionsService, IMobileVersionService versionService)
            : base(permissionsService)
        {
            this.versionService = versionService;
        }

        [HttpGet]
        public IActionResult IsAppOutdated([FromQuery] int version, string platform)
        {
            return Ok(versionService.IsAppOutdated(version, MobileTypeEnum.Pub, platform));
        }
    }
}
