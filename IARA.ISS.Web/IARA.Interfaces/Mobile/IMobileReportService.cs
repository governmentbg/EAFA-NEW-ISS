using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Mobile.Reports;

namespace IARA.Interfaces.Mobile
{
    public interface IMobileReportService
    {
        List<MobileReportNodeDTO> GetReports(int userId, bool isAdministrator);

        MobileReportDTO GetReport(int id, int userId, bool isAdministrator);

        MobileReportDTO GetReportByCode(string code, int userId, bool isAdministrator);

        string GetReportSQL(int id, int userId, bool isAdministrator);
    }
}
