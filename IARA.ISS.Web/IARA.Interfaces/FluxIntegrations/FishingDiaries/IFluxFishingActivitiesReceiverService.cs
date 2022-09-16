using System.Threading.Tasks;
using IARA.Flux.Models;

namespace IARA.Interfaces.Flux
{
    public interface IFluxFishingActivitiesReceiverService
    {
        Task<bool> ReportFishingActivities(FLUXFAReportMessageType report);
    }
}
