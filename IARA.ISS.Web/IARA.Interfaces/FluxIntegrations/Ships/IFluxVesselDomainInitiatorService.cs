using System.Threading.Tasks;
using IARA.Flux.Models;
using IARA.Interfaces.FluxIntegrations;

namespace IARA.Interfaces.Flux
{
    public interface IFluxVesselDomainInitiatorService : IBaseFluxService
    {
        Task<bool> ReceiveVesselQuery(FLUXVesselQueryMessageType query);
        Task<bool> VesselQueryReply(FLUXReportVesselInformationType response);
    }
}
