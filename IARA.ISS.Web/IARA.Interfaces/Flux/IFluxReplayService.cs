using System;
using System.Threading.Tasks;
using IARA.Common.Enums;

namespace IARA.Interfaces.Flux
{
    public interface IFluxReplayService : IService
    {
        Task<bool> ReplayOutgoing(Guid requestGuid, FluxFvmsDomainsEnum serviceType, string messageType = null);
        Task<bool> ReplayOutgoing(int id, FluxFvmsDomainsEnum serviceType, string messageType = null);
    }
}
