using System.Threading.Tasks;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.DTObjects.ReportViolations;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Transactions.Base;

namespace IARA.Mobile.Pub.Application.Transactions
{
    public class ReportViolationTransaction : BaseTransaction, IReportViolationTransaction
    {
        public ReportViolationTransaction(BaseTransactionProvider provider) : base(provider)
        {
        }

        public async Task<bool> ReportViolation(ReportViolationDto violation)
        {
            HttpResult result = await RestClient.PostAsync("ReportViolation/Report", violation);

            return result.IsSuccessful;
        }
    }
}
