using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Mobile.Application.DTObjects.Reports;
using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.Transactions.Base;

namespace IARA.Mobile.Pub.Application.Transactions
{
    public class ReportsTransaction : BaseTransaction, IReportsTransaction
    {
        public ReportsTransaction(BaseTransactionProvider provider)
            : base(provider) { }

        public async Task<List<ReportNodeDto>> GetAll()
        {
            HttpResult<List<ReportNodeDto>> result = await RestClient.GetAsync<List<ReportNodeDto>>("Report/GetAll");

            if (result.IsSuccessful)
            {
                return result.Content;
            }

            return null;
        }

        public async Task<ReportDto> Get(int id)
        {
            HttpResult<ReportDto> result = await RestClient.GetAsync<ReportDto>("Report/Get", new { id });

            if (result.IsSuccessful)
            {
                return result.Content;
            }

            return null;
        }

        public Task<ReportDto> GetByCode(string code)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<ReportColumnNameDto>> GetColumnNames(int reportId, List<ExecutionParamDto> parameters)
        {
            HttpResult<List<ReportColumnNameDto>> result =
                await RestClient.PostAsync<List<ReportColumnNameDto>>("Report/GetReportColumnNames", parameters, new { id = reportId });

            if (result.IsSuccessful)
            {
                return result.Content;
            }

            return null;
        }

        public async Task<ReportResultDto> ExecuteSQL(int reportId, List<ExecutionParamDto> parameters, int pageNumber, int pageSize)
        {
            HttpResult<ReportResultDto> result =
                await RestClient.PostAsync<ReportResultDto>("Report/ExecuteReportSQL", new { reportId, parameters, pageNumber, pageSize });

            if (result.IsSuccessful)
            {
                return result.Content;
            }

            return null;
        }
    }
}
