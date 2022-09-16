using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.Mobile.CatchRecords;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Mobile.Public
{
    [AreaRoute(AreaType.MobilePublic)]
    public class CatchRecordsController : BaseController
    {
        private readonly ICatchRecordsService service;
        private readonly IFileService fileService;

        public CatchRecordsController(IPermissionsService permissionsService, ICatchRecordsService service, IFileService fileService)
            : base(permissionsService)
        {
            this.service = service;
            this.fileService = fileService;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CatchRecordsRead)]
        public IActionResult GetAll([FromBody] GridRequestModel<CatchRecordPublicFilters> request)
        {
            MobileCatchRecordGroupDTO catches = this.service.GetCatchRecords(request.Filters, this.CurrentUser.ID, request.PageNumber, request.PageSize);
            return this.Ok(catches);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CatchRecordsAddRecords)]
        public IActionResult Create([FromForm] CatchRecordEditDTO request)
        {
            return this.Ok(this.service.CreateCatchRecord(request, this.CurrentUser.ID));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CatchRecordsEditRecords)]
        public IActionResult Edit([FromForm] CatchRecordEditDTO request)
        {
            this.service.UpdateCatchRecord(request, this.CurrentUser.ID);
            return this.Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.CatchRecordsDeleteRecords)]
        public IActionResult Delete([FromQuery] int id)
        {
            this.service.DeleteCatchRecord(id, this.CurrentUser.ID);
            return this.Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CatchRecordsRead)]
        public IActionResult Photo([FromQuery] int id)
        {
            if (this.service.HasAccessToFile(id, this.CurrentUser.ID))
            {
                DownloadableFileDTO file = this.fileService.GetFileForDownload(id);

                return this.File(file.Bytes, file.MimeType, file.FileName);
            }

            return this.NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CatchRecordsRead)]
        public IActionResult GalleryPhoto([FromQuery] int id)
        {
            if (this.service.HasAccessToFile(id, this.CurrentUser.ID))
            {
                DownloadableFileDTO file = this.fileService.GetResizedImage(id, maxWidth: 100, maxHeight: 100, compressionRate: 50);

                return this.File(file.Bytes, file.MimeType, file.FileName);
            }

            return this.NotFound();
        }
    }
}
