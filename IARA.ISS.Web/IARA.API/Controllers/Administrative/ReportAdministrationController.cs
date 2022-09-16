using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.Reports;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces.Reports;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class ReportAdministrationController : BaseController
    {
        private readonly IReportService reportService;
        private readonly IReportsManamementService reportsManamementService;
        private readonly IReportNomenclaturesService reportNomenclaturesService;

        public ReportAdministrationController(IPermissionsService permissionsService,
            IReportService reportService,
            IReportNomenclaturesService reportNomenclaturesService,
            IReportsManamementService reportsManamementService)
            : base(permissionsService)
        {
            this.reportService = reportService;
            this.reportsManamementService = reportsManamementService;
            this.reportNomenclaturesService = reportNomenclaturesService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportEditRecords)]
        public IActionResult GetAvailableNParameters()
        {
            return Ok(this.reportNomenclaturesService.GetAvailableReportNParameterNomenclatures());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportEditRecords)]
        public IActionResult GetReportGroups()
        {
            return Ok(this.reportNomenclaturesService.GetReportGroupsNomenclatures());
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ReportParameterAddRecords)]
        public IActionResult AddNParameter([FromBody] NReportParameterEditDTO nParameter)
        {
            return Ok(this.reportsManamementService.AddNParameter(nParameter));
        }

        [HttpPut]
        [CustomAuthorize(Permissions.ReportParameterEditRecords)]
        public IActionResult EditNParameter([FromBody] NReportParameterEditDTO nParameter)
        {
            this.reportsManamementService.EditNParameter(nParameter);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportParameterRead)]
        public IActionResult GetNParameter([FromQuery] int id)
        {
            return Ok(this.reportsManamementService.GetNParameter(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ReportParameterRead)]
        public IActionResult GetAllNParameters([FromBody] GridRequestModel<ReportParameterDefinitionFilters> gridRequestModel)
        {
            IQueryable<NReportParameterDTO> results = this.reportsManamementService.GetAllNParameters(gridRequestModel.Filters);
            return PageResult(results, gridRequestModel, false);
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.ReportParameterDeleteRecords)]
        public IActionResult DeleteNParameter([FromQuery] int id)
        {
            this.reportsManamementService.DeleteNParameter(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.ReportParameterRestoreRecords)]
        public IActionResult UndoDeletedNParameter([FromQuery] int id)
        {
            this.reportsManamementService.UndoDeletedNParameter(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportEditRecords)]
        public IActionResult GetReport([FromQuery] int id)
        {
            return Ok(this.reportService.GetReport(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ReportAddRecords)]
        public IActionResult AddReport([FromBody] ReportDTO report)
        {
            try
            {
                return Ok(this.reportsManamementService.AddReport(report));
            }
            catch (ArgumentException)
            {
                //TODO
                return ValidationFailedResult(null, ErrorCode.InvalidSqlQuery);
            }
        }

        [HttpPut]
        [CustomAuthorize(Permissions.ReportEditRecords)]
        public IActionResult EditReport([FromBody] ReportDTO report)
        {
            try
            {
                this.reportsManamementService.EditReport(report);
                return Ok();
            }
            catch (ArgumentException)
            {
                return ValidationFailedResult(null, ErrorCode.InvalidSqlQuery);
            }
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.ReportDeleteRecords)]
        public IActionResult DeleteReport([FromQuery] int id)
        {
            this.reportsManamementService.DeleteReport(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.ReportRestoreRecords)]
        public IActionResult UndoDeletedReport([FromQuery] int id)
        {
            this.reportsManamementService.UndoDeletedReport(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportEditRecords)]
        public IActionResult GetGroup([FromQuery] int id)
        {
            return Ok(this.reportService.GetGroup(id));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ReportAddRecords)]
        public IActionResult AddGroup([FromBody] ReportGroupDTO group)
        {
            return Ok(this.reportsManamementService.AddGroup(group));
        }

        [HttpPut]
        [CustomAuthorize(Permissions.ReportEditRecords)]
        public IActionResult EditGroup([FromBody] ReportGroupDTO group)
        {
            this.reportsManamementService.EditGroup(group);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.ReportDeleteRecords)]
        public IActionResult DeleteGroup([FromQuery] int id)
        {
            this.reportsManamementService.DeleteGroup(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.ReportRestoreRecords)]
        public IActionResult UndoDeletedGroup([FromQuery] int id)
        {
            this.reportsManamementService.UndoDeletedGroup(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportRead, Permissions.ReportAddRecords)]
        public IActionResult GetReportNodes()
        {
            if (CurrentUser.Permissions.Contains(nameof(Permissions.ReportAddRecords)))
            {
                return Ok(this.reportService.GetAllReportNodes());
            }
            else
            {
                return Ok(this.reportService.GetReportNodes(this.CurrentUser.ID));
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportRead)]
        public IActionResult GetTableNodes()
        {
            return Ok(this.reportService.GetTableNodes());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportRead, Permissions.ReportAddRecords)]
        public IActionResult GetExecuteReport([FromQuery] int id)
        {
            bool hasAccess = this.reportService.HasUserAccessToReport(id, this.CurrentUser.ID);
            if (CurrentUser.Permissions.Contains(nameof(Permissions.ReportAddRecords))
                || hasAccess)
            {
                try
                {
                    return Ok(this.reportService.GetExecuteReport(id, this.CurrentUser.ID));
                }
                catch (ArgumentException)
                {
                    return ValidationFailedResult(null, ErrorCode.InvalidSqlQuery);
                }
            }
            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportRead, Permissions.ReportAddRecords)]
        public IActionResult GetExecuteReportParameters([FromQuery] int id)
        {
            bool hasAccess = this.reportService.HasUserAccessToReport(id, this.CurrentUser.ID);
            if (CurrentUser.Permissions.Contains(nameof(Permissions.ReportAddRecords))
                 || hasAccess)
            {
                try
                {
                    return Ok(this.reportService.GetExecuteReportParameters(id, this.CurrentUser.ID));
                }
                catch (ArgumentException)
                {
                    return ValidationFailedResult(null, ErrorCode.InvalidSqlQuery);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ReportRead, Permissions.ReportAddRecords)]
        public IActionResult ExecutePagedQuery([FromBody] ReportGridRequestDTO report)
        {
            bool hasAccess = this.reportService.HasUserAccessToReport(report.ReportId, this.CurrentUser.ID);

            if (hasAccess)
            {
                try
                {
                    report.UserId = CurrentUser.ID;

                    return Ok(this.reportService.ExecutePagedSql(report));
                }
                catch (ArgumentException)
                {
                    return ValidationFailedResult(null, ErrorCode.InvalidSqlQuery);
                }
            }
            else if (CurrentUser.Permissions.Contains(nameof(Permissions.ReportAddRecords)))
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
        [CustomAuthorize(Permissions.ReportAddRecords, Permissions.ReportEditRecords)]
        public IActionResult ExecuteRawSql([FromBody] ExecutionReportInfoDTO report)
        {
            try
            {
                return Ok(this.reportService.ExecuteRawSql(report.SqlQuery, report.Parameters, CurrentUser.ID));
            }
            catch (ArgumentException)
            {
                return ValidationFailedResult(null, ErrorCode.InvalidSqlQuery);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ReportRead)]
        public IActionResult GenerateCSV([FromQuery] int reportId, [FromBody] List<ExecutionParamDTO> parameters)
        {
            try
            {
                Stream stream = this.reportService.GenerateCSV(reportId, parameters, CurrentUser.ID, out string reportName);

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{reportName}.xlsx");
            }
            catch (ArgumentException ex)
            {
                return ValidationFailedResult(null, ErrorCode.InvalidSqlQuery);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ReportRead, Permissions.ReportAddRecords)]
        public async Task<IActionResult> DownloadReport([FromQuery] int id, [FromBody] List<ExecutionParamDTO> parameters)
        {
            bool hasAccess = this.reportService.HasUserAccessToReport(id, this.CurrentUser.ID);
            if (CurrentUser.Permissions.Contains(nameof(Permissions.ReportAddRecords))
                || hasAccess)
            {
                Stream stream = await this.reportService.RunReport(id, parameters, CurrentUser.ID, out string reportName);
                return File(stream, "application/pdf", $"{reportName}.pdf");
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ReportRead, Permissions.ReportAddRecords)]
        public IActionResult GetColumnNames([FromBody] ExecutionReportInfoDTO report)
        {
            if (CurrentUser.Permissions.Contains(nameof(Permissions.ReportAddRecords))
                && !string.IsNullOrEmpty(report.SqlQuery))
            {
                try
                {
                    return Ok(this.reportService.GetColumnNames(report.SqlQuery, report.Parameters, CurrentUser.ID));
                }
                catch (ArgumentException)
                {
                    return ValidationFailedResult(null, ErrorCode.InvalidSqlQuery);
                }
            }
            else
            {
                try
                {
                    return Ok(this.reportService.GetColumnNames(report.ReportId, report.Parameters, CurrentUser.ID));
                }
                catch (ArgumentException)
                {
                    return ValidationFailedResult(null, ErrorCode.InvalidSqlQuery);
                }
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportRead)]
        public IActionResult GetReportSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = reportService.GetSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportParameterRead)]
        public IActionResult GetReportParametersSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = reportService.GetReportParametersSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportParameterRead)]
        public IActionResult GetParametersSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = reportService.GetParametersSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ReportParameterRead)]
        public IActionResult GetReportGroupsSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = reportService.GetReportGroupsSimpleAudit(id);
            return Ok(audit);
        }
    }
}
