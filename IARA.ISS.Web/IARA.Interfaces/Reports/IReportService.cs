using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IARA.Common.GridModels;
using IARA.DomainModels.DTOModels.Reports;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces.Reports
{
    public interface IReportService
    {
        BaseGridResultModel<IDictionary<string, object>> ExecutePagedSql(ReportGridRequestDTO request);

        IEnumerable<IDictionary<string, object>> ExecuteRawSql(string sqlQuery, List<ExecutionParamDTO> parameters, int? userId);

        Stream GenerateCSV(int reportId, List<ExecutionParamDTO> parameters, int? userId, out string reportName);

        List<ReportNodeDTO> GetAllReportNodes();

        HashSet<ReportSchema> GetColumnNames(string sqlQuery, List<ExecutionParamDTO> parameters, int? userId);

        HashSet<ReportSchema> GetColumnNames(int reportId, List<ExecutionParamDTO> parameters, int? userId);

        ExecuteReportDTO GetExecuteReport(int id, int? userId);

        List<ReportParameterExecuteDTO> GetExecuteReportParameters(int reportId, int? userId);

        ReportGroupDTO GetGroup(int id);

        ReportDTO GetReport(int id);

        List<ReportNodeDTO> GetReportNodes(int? userId);

        List<TableNodeDTO> GetTableNodes();

        bool HasUserAccessToReport(int reportId, int? userId);

        Task<Stream> RunReport(int id, List<ExecutionParamDTO> parameters, int? userId, out string reportName);

        SimpleAuditDTO GetReportGroupsSimpleAudit(int id);

        SimpleAuditDTO GetReportParametersSimpleAudit(int id);

        SimpleAuditDTO GetParametersSimpleAudit(int id);

        SimpleAuditDTO GetSimpleAudit(int id);
    }
}
