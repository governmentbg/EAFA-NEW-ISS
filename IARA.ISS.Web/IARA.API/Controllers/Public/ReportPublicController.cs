using IARA.Interfaces.Reports;
using IARA.Security;
using IARA.WebAPI.Controllers.Common;
using IARA.WebHelpers;

namespace IARA.WebAPI.Controllers.Public
{
    [AreaRoute(AreaType.Public)]
    public class ReportPublicController : BasePublicReportController
    {
        public ReportPublicController(IPermissionsService permissionsService, IReportService reportService)
            : base(permissionsService, reportService)
        { }
    }
}
