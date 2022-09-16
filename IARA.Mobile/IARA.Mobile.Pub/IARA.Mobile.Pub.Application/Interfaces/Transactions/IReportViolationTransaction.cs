using IARA.Mobile.Pub.Application.DTObjects.ReportViolations;
using System.Threading.Tasks;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface IReportViolationTransaction
    {
        Task<bool> ReportViolation(ReportViolationDto violation);
    }
}
