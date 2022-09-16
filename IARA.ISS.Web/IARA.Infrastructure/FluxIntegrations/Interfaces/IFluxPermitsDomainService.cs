using System;
using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.FLUXVMSRequests;
using IARA.Flux.Models;

namespace IARA.Infrastructure.FluxIntegrations.Interfaces
{
    public interface IFluxPermitsDomainService
    {
        Task<bool> SendFlapRequest(Tuple<FLUXFLAPRequestMessageType, FluxFlapRequestEditDTO> request);

        void ReceiveFlapResponse(FLUXFLAPResponseMessageType response);

        void ReceiveFlapRequest(FLUXFLAPRequestMessageType request);
    }
}
