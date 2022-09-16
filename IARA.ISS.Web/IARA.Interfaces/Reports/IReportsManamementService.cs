using System.Linq;
using IARA.DomainModels.DTOModels.Reports;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces.Reports
{
    public interface IReportsManamementService
    {
        int AddGroup(ReportGroupDTO inputGroup);
        int AddNParameter(NReportParameterEditDTO inputNParameter);
        int AddReport(ReportDTO inputReport);
        void DeleteGroup(int id);
        void DeleteNParameter(int id);
        void DeleteReport(int id);
        void EditGroup(ReportGroupDTO inputGroup);
        void EditNParameter(NReportParameterEditDTO inputNParameter);
        void EditReport(ReportDTO inputReport);
        IQueryable<NReportParameterDTO> GetAllNParameters(ReportParameterDefinitionFilters filters);
        NReportParameterEditDTO GetNParameter(int id);
        void UndoDeletedGroup(int id);
        void UndoDeletedNParameter(int id);
        void UndoDeletedReport(int id);
    }
}
