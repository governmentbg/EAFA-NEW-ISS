using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Mobile.Reports;
using IARA.DomainModels.DTOModels.Reports;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces.Mobile;
using IARA.Interfaces.Reports;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebAPI.Controllers.Common;
using IARA.WebHelpers;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Mobile.Public
{
    [AreaRoute(AreaType.MobilePublic)]
    public class ReportController : BasePublicReportController
    {
        private readonly IMobileReportService mobileReportService;

        public ReportController(IPermissionsService permissionsService, IMobileReportService mobileReportService, IReportService reportService)
            : base(permissionsService, reportService)
        {
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

            MobileReportDTO report = this.mobileReportService.GetReport(id, this.CurrentUser.ID, isAdmin);
            return this.Ok(report);
        }

        [HttpPost]
        public IActionResult GetReportColumnNames([FromQuery] int id, [FromBody] List<ExecutionParamDTO> parameters)
        {
            return base.GetColumnNames(new ExecutionReportInfoDTO
            {
                ReportId = id,
                Parameters = parameters
            });
        }

        [HttpPost]
        public IActionResult ExecuteReportSQL([FromBody] ReportMobileRequestDTO request)
        {
            return base.ExecutePagedQuery(new ReportGridRequestDTO()
            {
                Parameters = request.Parameters,
                PageNumber = request.PageNumber,
                ReportId = request.ReportId,
                PageSize = request.PageSize,
                UserId = this.CurrentUser?.ID
            });
        }
    }
}
