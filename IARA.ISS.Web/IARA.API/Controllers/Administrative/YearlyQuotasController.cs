using System.Collections.Generic;
using System.IO;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.CatchQuotas;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.RequestModels;
using IARA.Excel.Tools.Models;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class YearlyQuotasController : BaseAuditController
    {
        private readonly IYearlyQuotasService service;
        private readonly IFileService fileService;

        public YearlyQuotasController(IYearlyQuotasService service,
                                      IPermissionsService permissionsService,
                                      IFileService fileService)
            : base(permissionsService)
        {
            this.service = service;
            this.fileService = fileService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.YearlyQuotasRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            return Ok(service.GetSimpleAudit(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.YearlyQuotasAddRecords)]
        public IActionResult Add([FromBody] YearlyQuotaEditDTO entry)
        {
            try
            {
                this.service.Add(entry);
                return Ok();
            }
            catch
            {
                return ValidationFailedResult(null, ErrorCode.AlreadySubmitted);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.YearlyQuotasRead)]
        public IActionResult GetAll([FromBody] GridRequestModel<YearlyQuotasFilters> request)
        {
            var result = service.GetAll(request.Filters);

            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.YearlyQuotasRead)]
        public IActionResult Get([FromQuery] int id)
        {
            var result = service.Get(id);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.YearlyQuotasRead)]
        public IActionResult GetChangeHistory([FromBody] IEnumerable<int> ids)
        {
            var result = service.GetHistoryForIds(ids);

            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.YearlyQuotasRead)]
        public IActionResult GetLastYearsQuota([FromQuery] int newQuotaId)
        {
            var result = service.GetLastYearsQuota(newQuotaId);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.YearlyQuotasEditRecords)]
        public IActionResult Edit([FromForm] YearlyQuotaEditDTO entry)
        {
            service.Edit(entry);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.YearlyQuotasEditRecords)]
        public IActionResult Transfer([FromQuery] int newQuotaId, [FromQuery] int oldQuotaId, [FromQuery] int transferValue, [FromQuery] string basis)
        {
            service.Transfer(newQuotaId, oldQuotaId, transferValue, basis);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipQuotasRead)]
        public IActionResult GetYearlyQuotasForList()
        {
            var result = service.GetYearlyQuotasForList();
            return Ok(result);
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.YearlyQuotasDeleteRecords)]
        public IActionResult Delete([FromQuery] int id)
        {
            service.Delete(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.YearlyQuotasRestoreRecords)]
        public IActionResult Restore([FromQuery] int id)
        {
            service.Restore(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.YearlyQuotasRead)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.YearlyQuotasRead)]
        public IActionResult DownloadYearlyQuotaExcel([FromBody] ExcelExporterRequestModel<YearlyQuotasFilters> request)
        {
            Stream stream = service.DownloadYearlyQuotaExcel(request);
            return ExcelFile(stream, request.Filename);
        }
    }
}
