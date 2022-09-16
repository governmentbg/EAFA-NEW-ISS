using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Dashboard;

namespace IARA.Interfaces
{
    public interface IDashboardService : IService
    {
        StatusCountReportDataDTO GetStatusCountReportData(int? currentUserId, bool isTickets, PageCodeEnum[] pageCodes);
        List<TypesCountReportDTO> GetTypesCountReport(int? currentUserId = null);
        List<TicketTypesCountReportDTO> GetTicketTypesCountReport();
    }
}
