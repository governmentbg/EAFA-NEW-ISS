using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.ScientificFishing;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Mobile.Public
{
    [AreaRoute(AreaType.MobilePublic)]
    public class ScientificFishingController : BaseController
    {
        private readonly IScientificFishingMobileService service;
        private readonly IScientificFishingService scientificFishingService;
        private readonly IFileService fileService;

        public ScientificFishingController(
            IScientificFishingMobileService service,
            IScientificFishingService scientificFishingService,
            IFileService fileService,
            IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
            this.scientificFishingService = scientificFishingService;
            this.fileService = fileService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingRead)]
        public IActionResult GetAllPermits()
        {
            return this.Ok(this.service.GetAllPermits(this.CurrentUser.ID));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ScientificFishingRead)]
        public IActionResult AddOuting([FromBody] ScientificFishingOutingDTO outing)
        {
            int id = this.service.AddOuting(outing, this.CurrentUser.ID);
            return this.Ok(id);
        }

        [HttpPut]
        [CustomAuthorize(Permissions.ScientificFishingRead)]
        public IActionResult EditOuting([FromBody] ScientificFishingOutingDTO outing)
        {
            this.service.EditOuting(outing, this.CurrentUser.ID);
            return this.Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.ScientificFishingRead)]
        public IActionResult DeleteOuting([FromQuery] int id)
        {
            this.service.DeleteOuting(id, this.CurrentUser.ID);
            return this.Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingRead)]
        public IActionResult DownloadFile(int id)
        {
            if (this.scientificFishingService.HasUserAccessToPermitFile(this.CurrentUser.ID, id))
            {
                DownloadableFileDTO file = this.fileService.GetFileForDownload(id);
                return this.File(file.Bytes, file.MimeType, file.FileName);
            }
            return this.NotFound();
        }
    }
}
