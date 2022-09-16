using System.Threading.Tasks;
using IARA.Flux.Models;

namespace IARA.Interfaces.FluxIntegrations
{
    public interface IBaseFluxService
    {
        Task<bool> ReceiveResponse(FLUXResponseMessageType response);
    }
}
