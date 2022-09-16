using System.Threading.Tasks;
using IARA.Flux.Models;
using IARA.Interfaces.FluxIntegrations;

namespace IARA.Interfaces.Flux.PermitsAndCertificates
{
    public interface IFluxPermitsDomainInitiatorService : IBaseFluxService
    {
        Task<bool> ReceiveFlapResponse(FLUXFLAPResponseMessageType response);
        Task<bool> ReceiveFlapRequest(FLUXFLAPRequestMessageType request);
    }
}
