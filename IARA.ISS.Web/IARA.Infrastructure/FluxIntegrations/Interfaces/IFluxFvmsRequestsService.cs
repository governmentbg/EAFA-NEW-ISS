using IARA.FVMSModels.Common;

namespace IARA.Infrastructure.FluxIntegrations.Interfaces
{
    public interface IFluxFvmsRequestsService
    {
        void AddFluxFvmsRequest(FluxFvmsRequest request);
        void AddFluxFvmsResponse(FluxFvmsResponse response);
    }
}
