using System.Threading.Tasks;
using IARA.Flux.Models;
using IARA.Interfaces.FluxIntegrations;

namespace IARA.Interfaces.Flux
{
    public interface IFluxFishingActivitiesInitiatorService : IBaseFluxService
    {
        Task<bool> FAReportReceived(FLUXFAReportMessageType reponse);
        Task<bool> ReceiveFAQuery(FLUXFAQueryMessageType query);
    }
}
