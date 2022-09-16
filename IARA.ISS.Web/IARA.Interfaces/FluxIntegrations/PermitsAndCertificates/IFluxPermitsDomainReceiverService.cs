using System;
using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.FLUXVMSRequests;
using IARA.Flux.Models;

namespace IARA.Interfaces.Flux.PermitsAndCertificates
{
    public interface IFluxPermitsDomainReceiverService
    {
        Task<bool> SendFlapRequest(Tuple<FLUXFLAPRequestMessageType, FluxFlapRequestEditDTO> request);
    }
}
