using IARA.DomainModels.DTOModels.FLUXVMSRequests;
using IARA.Flux.Models;

namespace IARA.Interfaces.FluxIntegrations.FishingDiaries
{
    public interface IFishingActivitiesDomainMapper
    {
        FLUXFAQueryMessageType MapQuery(FluxFAQueryRequestEditDTO request);
    }
}
