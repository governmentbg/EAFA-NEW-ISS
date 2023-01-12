using System.Threading.Tasks;
using IARA.Mobile.Pub.Application.DTObjects.ReportViolations;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface IReportViolationTransaction
    {
        Task<bool> ReportViolation(ReportViolationDto violation);
    }
}
