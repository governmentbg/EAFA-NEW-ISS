using System;
using IARA.DomainModels.DTOModels.FLUXVMSRequests;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;

namespace IARA.Interfaces.FluxIntegrations.PermitsAndCertificates
{
    public interface IFlapDomainMapper 
    {
        FLUXFLAPRequestMessageType MapRequestToFlux(FluxFlapRequestEditDTO request, Guid referenceId, ReportPurposeCodes purpose);

        FluxFlapRequestEditDTO MapFluxToRequest(FLUXFLAPRequestMessageType request);
    }
}
