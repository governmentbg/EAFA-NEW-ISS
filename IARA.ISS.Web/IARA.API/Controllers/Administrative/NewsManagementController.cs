using System.Linq;
using IARA.DomainModels.DTOModels.NewsManagment;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using IARA.Security.Permissions;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class NewsManagementController : BaseAuditController
    {
        private readonly INewsManagementService service;
        private readonly IFileService fileService;

        public NewsManagementController(INewsManagementService service,
                                        IFileService fileService,
                                        IPermissionsService permissionService) 
            : base(permissionService)
        {
            this.service = service;
            this.fileService = fileService;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.NewsManagementRead)]
        public IActionResult GetAll([FromBody] GridRequestModel<NewsManagmentFilters> gridRequestModel)
        {
            IQueryable<NewsManagementDTO> results = service.GetAll(gridRequestModel.Filters);
            return PageResult(results, gridRequestModel, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.NewsManagementRead)]
        public IActionResult Get([FromQuery] int id)
        {
            NewsManagementEditDTO news = service.Get(id);
            return Ok(news);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.NewsManagementAddRecords)]
        public IActionResult Add([FromForm] NewsManagementEditDTO news)
        {
            int id = service.Add(news);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.NewsManagementEditRecords)]
        public IActionResult Edit([FromForm] NewsManagementEditDTO news)
        {
            service.Edit(news);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.NewsManagementDeleteRecords)]
        public IActionResult DeleteNews([FromQuery] int id)
        {
            service.DeleteNews(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.NewsManagementRestoreRecords)]
        public IActionResult UndoDeletedNews([FromQuery] int id)
        {
            service.UndoDeletedNews(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.NewsManagementRead)]
        public IActionResult GetMainImage([FromQuery] int id)
        {
            string photo = service.GetMainImage(id);
            return Ok(photo);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.NewsManagementRead)]
        public IActionResult DownloadFile([FromQuery] int id)
        {
            DownloadableFileDTO file = this.fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.NewsManagementRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }
    }
}
