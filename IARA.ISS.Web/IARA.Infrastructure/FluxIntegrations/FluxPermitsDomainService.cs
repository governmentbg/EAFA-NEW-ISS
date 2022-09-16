using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.FLUXVMSRequests;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Interfaces.Flux.PermitsAndCertificates;
using IARA.Interfaces.FluxIntegrations.PermitsAndCertificates;

namespace IARA.Infrastructure.FluxIntegration
{
    public class FluxPermitsDomainService : BaseService, IFluxPermitsDomainService
    {
        private readonly IFluxPermitsDomainReceiverService permitsDomainReceiverService;
        private readonly IFlapDomainMapper flapDomainMapper;

        public FluxPermitsDomainService(IARADbContext db,
                                        IFluxPermitsDomainReceiverService permitsDomainReceiverService,
                                        IFlapDomainMapper flapDomainMapper)
            : base(db)
        {
            this.permitsDomainReceiverService = permitsDomainReceiverService;
            this.flapDomainMapper = flapDomainMapper;
        }

        /// <summary>
        /// Заявки за данни за разрешително за риболов
        /// Fishing Licence Authorization and Permit (FLAP)
        /// </summary>
        public Task<bool> SendFlapRequest(Tuple<FLUXFLAPRequestMessageType, FluxFlapRequestEditDTO> request)
        {
            return permitsDomainReceiverService.SendFlapRequest(request);
        }

        /// <summary>
        /// Получаване на отговор с данни
        /// </summary>
        public void ReceiveFlapResponse(FLUXFLAPResponseMessageType response)
        {
            Fluxflaprequest request = (from flap in Db.Fluxflaprequests
                                       join flux in Db.Fluxfvmsrequests on flap.FluxfvmsrequestId equals flux.Id
                                       where flux.RequestUuid == Guid.Parse(response.FLUXResponseDocument.ReferencedID.Value)
                                       select flap).First();

            request.ResponseContent = JsonSerializer.Serialize(response);

            Db.SaveChanges();
        }

        /// <summary>
        /// Получаване на заявка
        /// </summary>
        public void ReceiveFlapRequest(FLUXFLAPRequestMessageType request)
        {
            Fluxflaprequest entry = new()
            {
                FluxfvmsrequestId = (from flux in Db.Fluxfvmsrequests
                                     where flux.RequestUuid == Guid.Parse(request.FLUXReportDocument.ID[0].Value)
                                     select flux.Id).First(),
                MdrFlapRequestPurposeId = (from purpose in Db.MdrFlapRequestPurposes
                                           where purpose.Code == request.FLAPRequestDocument.PurposeCode.Value
                                           select purpose.Id).First(),
                RequestContent = JsonSerializer.Serialize(flapDomainMapper.MapFluxToRequest(request)),
                ShipIdentifierType = request.FLAPDocument.VesselID[0].schemeID,
                ShipIdentifier = request.FLAPDocument.VesselID[0].Value,
                ShipName = "N/A"
            };

            Db.Fluxflaprequests.Add(entry);
            Db.SaveChanges();
        }
    }
}
