using System.Threading.Tasks;

namespace IARA.RegixAbstractions.Interfaces
{
    public interface IRegixConclusionsService
    {
        Task<bool> FinalDecision(int applicationId, int applicationHistoryId);
    }
}