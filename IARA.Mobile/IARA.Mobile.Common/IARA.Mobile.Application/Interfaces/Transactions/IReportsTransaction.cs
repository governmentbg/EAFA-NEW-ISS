using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Mobile.Application.DTObjects.Reports;

namespace IARA.Mobile.Application.Interfaces.Transactions
{
    public interface IReportsTransaction
    {
        Task<List<ReportNodeDto>> GetAll();

        Task<ReportDto> Get(int id);

        Task<ReportDto> GetByCode(string code);

        Task<List<ReportColumnNameDto>> GetColumnNames(int reportId, List<ExecutionParamDto> parameters);

        Task<ReportResultDto> ExecuteSQL(int reportId, List<ExecutionParamDto> parameters, int pageNumber, int pageSize);
    }
}
