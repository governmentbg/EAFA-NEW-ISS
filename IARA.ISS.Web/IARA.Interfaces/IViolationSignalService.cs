using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.ReportViolations;

namespace IARA.Interfaces
{
    public interface IViolationSignalService
    {
        Task<bool> EnqueueSignalForViolationEmail(ReportViolationDTO violation, int userId);
    }
}
