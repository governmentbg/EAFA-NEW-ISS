using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.Reports;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces.Mobile;
using IARA.Interfaces.Reports;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Mobile.Administrative
{
    [AreaRoute(AreaType.MobileAdministrative)]
    public class ReportController : BaseController
    {
        private readonly IMobileReportService mobileReportService;
        private readonly IReportService reportService;

        public ReportController(IPermissionsService permissionsService, IMobileReportService mobileReportService, IReportService reportService)
            : base(permissionsService)
        {
            this.reportService = reportService;
            this.mobileReportService = mobileReportService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportRead)]
        public IActionResult GetAll()
        {
            bool isAdmin = this.CurrentUser.Permissions.Contains(nameof(Permissions.ReportAddRecords));

            return this.Ok(this.mobileReportService.GetReports(this.CurrentUser.ID, isAdmin));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportRead)]
        public IActionResult Get([FromQuery] int id)
        {
            bool isAdmin = this.CurrentUser.Permissions.Contains(nameof(Permissions.ReportAddRecords));

            return this.Ok(this.mobileReportService.GetReport(id, this.CurrentUser.ID, isAdmin));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportRead)]
        public IActionResult GetByCode([FromQuery] string code)
        {
            bool isAdmin = this.CurrentUser.Permissions.Contains(nameof(Permissions.ReportAddRecords));

            return this.Ok(this.mobileReportService.GetReportByCode(code, this.CurrentUser.ID, isAdmin));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportRead)]
        public IActionResult GetTableNodes()
        {
            return this.Ok(this.reportService.GetTableNodes());
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ReportRead)]
        public IActionResult GetReportColumnNames([FromQuery] int id, [FromBody] List<ExecutionParamDTO> parameters)
        {
            bool isAdmin = this.CurrentUser.Permissions.Contains(nameof(Permissions.ReportAddRecords));

            string sql = this.mobileReportService.GetReportSQL(id, this.CurrentUser.ID, isAdmin);
            return this.Ok(this.reportService.GetColumnNames(sql, parameters, null));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ReportRead)]
        public IActionResult ExecuteReportSQL([FromBody] ReportMobileRequestDTO request)
        {
            bool hasAccess = this.CurrentUser.Permissions.Contains(nameof(Permissions.ReportAddRecords))
                || this.reportService.HasUserAccessToReport(request.ReportId, this.CurrentUser.ID);

            if (hasAccess)
            {
                try
                {
                    ReportGridRequestDTO report = new ReportGridRequestDTO()
                    {
                        Parameters = request.Parameters,
                        PageNumber = request.PageNumber,
                        ReportId = request.ReportId,
                        PageSize = request.PageSize,
                        UserId = this.CurrentUser.ID
                    };
                    ReportDTO reportDefinition = this.reportService.GetReport(report.ReportId);
                    report.SqlQuery = reportDefinition.ReportSQL;

                    return this.Ok(this.reportService.ExecutePagedSql(report));
                }
                catch (ArgumentException)
                {
                    return this.ValidationFailedResult(null, ErrorCode.InvalidSqlQuery);
                }
            }

            return this.NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportRead)]
        public async Task<IActionResult> DownloadReport([FromQuery] int id, [FromQuery] List<ExecutionParamDTO> parameters)
        {
            Stream stream = await this.reportService.RunReport(id, parameters, this.CurrentUser.ID, out string reportName);
            return this.File(stream, "application/pdf", $"{reportName}.pdf");
        }
    }
}
