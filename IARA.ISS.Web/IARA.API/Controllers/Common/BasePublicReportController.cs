using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.Reports;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces.Reports;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Common
{
    public abstract class BasePublicReportController : BaseController
    {
        protected readonly IReportService reportService;

        protected BasePublicReportController(IPermissionsService permissionsService, IReportService reportService)
           : base(permissionsService)
        {
            this.reportService = reportService;
        }

        [HttpGet]
        public IActionResult GetGroup([FromQuery] int id)
        {
            return Ok(this.reportService.GetGroup(id));
        }

        [HttpGet]
        public IActionResult GetReportNodes()
        {
            return Ok(this.reportService.GetReportNodes(this.CurrentUser?.ID));
        }

        [HttpGet]
        public IActionResult GetExecuteReport([FromQuery] int id)
        {
            bool hasAccess = this.reportService.HasUserAccessToReport(id, this.CurrentUser?.ID);

            if (hasAccess)
            {
                return Ok(this.reportService.GetExecuteReport(id, this.CurrentUser?.ID));
            }

            return NotFound();
        }

        [HttpGet]
        public IActionResult GetExecuteReportParameters([FromQuery] int id)
        {
            bool hasAccess = this.reportService.HasUserAccessToReport(id, this.CurrentUser?.ID);

            if (hasAccess)
            {
                return Ok(this.reportService.GetExecuteReportParameters(id, this.CurrentUser?.ID));
            }

            return NotFound();
        }

        [HttpPost]
        public IActionResult ExecutePagedQuery([FromBody] ReportGridRequestDTO report)
        {
            bool hasAccess = this.reportService.HasUserAccessToReport(report.ReportId, this.CurrentUser?.ID);

            if (hasAccess)
            {
                try
                {
                    report.UserId = CurrentUser?.ID;
                    ReportDTO reportDefinition = this.reportService.GetReport(report.ReportId);
                    report.SqlQuery = reportDefinition.ReportSQL;

                    return Ok(this.reportService.ExecutePagedSql(report));
                }
                catch (ArgumentException)
                {
                    return ValidationFailedResult(null, ErrorCode.InvalidSqlQuery);
                }
            }
            else if (CurrentUser != null && CurrentUser.Permissions.Contains(nameof(Permissions.ReportAddRecords)))
            {
                try
                {
                    report.UserId = CurrentUser.ID;

                    if (string.IsNullOrEmpty(report.SqlQuery))
                    {
                        ReportDTO reportDefinition = this.reportService.GetReport(report.ReportId);
                        report.SqlQuery = reportDefinition.ReportSQL;
                    }

                    return Ok(this.reportService.ExecutePagedSql(report));
                }
                catch (ArgumentException)
                {
                    return ValidationFailedResult(null, ErrorCode.InvalidSqlQuery);
                }
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        public IActionResult GenerateCSV([FromQuery] int reportId, [FromBody] List<ExecutionParamDTO> parameters)
        {
            try
            {
                if (this.reportService.HasUserAccessToReport(reportId, CurrentUser?.ID))
                {
                    Stream stream = this.reportService.GenerateCSV(reportId, parameters, CurrentUser?.ID, out string reportName);

                    return File(stream, "text/csv", $"{reportName}.csv");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (ArgumentException)
            {
                return ValidationFailedResult(null, ErrorCode.InvalidSqlQuery);
            }
        }

        [HttpPost]
        public IActionResult GetColumnNames([FromBody] ExecutionReportInfoDTO report)
        {
            return Ok(this.reportService.GetColumnNames(report.ReportId, report.Parameters, CurrentUser?.ID));
        }


        [HttpPost]
        public async Task<IActionResult> DownloadReport([FromQuery] int id, [FromBody] List<ExecutionParamDTO> parameters)
        {
            bool hasAccess = this.reportService.HasUserAccessToReport(id, this.CurrentUser.ID);

            if (hasAccess)
            {
                Stream stream = await this.reportService.RunReport(id, parameters, CurrentUser.ID, out string reportName);
                return File(stream, "application/pdf", $"{reportName}.pdf");
            }

            return NotFound();
        }
    }
}
