using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.FLUXVMSRequests;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.Infrastructure.FluxIntegration;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Interfaces.Flux.PermitsAndCertificates;
using TL.BatchWorkers.Interfaces;
using TLTTS.Common.ConfigModels;

namespace IARA.Infrastructure.FluxIntegrations.QueueServices
{
    public class FluxPermitsQueueService : BaseFluxService, IFluxPermitsDomainInitiatorService, IFluxPermitsDomainReceiverService
    {
        private readonly IAsyncWorkerTaskQueue<Tuple<FLUXFLAPRequestMessageType, FluxFlapRequestEditDTO>, bool> flapRequestsQueue;
        private readonly IAsyncWorkerTaskQueue<FLUXFLAPResponseMessageType, bool> flapResponsesQueue;
        private readonly IAsyncWorkerTaskQueue<FLUXFLAPRequestMessageType, bool> incomingFlapRequestsQueue;

        public FluxPermitsQueueService(ScopedServiceProviderFactory scopedServiceProviderFactory, ConnectionStrings connection)
            : base(scopedServiceProviderFactory, connection, nameof(responseQueue))
        {
            this.flapRequestsQueue = WorkerCreationUtils.CreateWorkerQueue<Tuple<FLUXFLAPRequestMessageType, FluxFlapRequestEditDTO>, bool>(FlapRequestWorker, connection, nameof(flapRequestsQueue));
            this.flapResponsesQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXFLAPResponseMessageType, bool>(ProcessFlapResponse, connection, nameof(flapResponsesQueue));
            this.incomingFlapRequestsQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXFLAPRequestMessageType, bool>(ProcessFlapRequest, connection, nameof(incomingFlapRequestsQueue));
        }

        public Task<bool> SendFlapRequest(Tuple<FLUXFLAPRequestMessageType, FluxFlapRequestEditDTO> request)
        {
            return flapRequestsQueue.Enqueue(request);
        }

        public Task<bool> ReceiveFlapResponse(FLUXFLAPResponseMessageType response)
        {
            return flapResponsesQueue.Enqueue(response);
        }

        public Task<bool> ReceiveFlapRequest(FLUXFLAPRequestMessageType request)
        {
            return incomingFlapRequestsQueue.Enqueue(request);
        }

        private async Task<bool> FlapRequestWorker(Tuple<FLUXFLAPRequestMessageType, FluxFlapRequestEditDTO> request, CancellationToken token)
        {
            bool result = await Retry(() => SendMessageToFlux(request.Item1.FLUXReportDocument,
                                                              request.Item1,
                                                              nameof(FluxFvmsDomainsEnum.FLAPDomain),
                                                              FluxEndpoints.FLUX_FLAP_REQUEST));

            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IARADbContext db = serviceProvider.GetRequiredService<IARADbContext>();

            Fluxflaprequest entry = new()
            {
                FluxfvmsrequestId = (from flux in db.Fluxfvmsrequests
                                     where flux.RequestUuid == (Guid)request.Item1.FLUXReportDocument.ID[0]
                                     select flux.Id).First(),
                MdrFlapRequestPurposeId = (from purpose in db.MdrFlapRequestPurposes
                                           where purpose.Code == request.Item2.RequestPurposeCode
                                           select purpose.Id).First(),
                RequestContent = JsonSerializer.Serialize(request.Item2),
                ShipId = request.Item2.Ship.ShipId.Value,
                ShipIdentifierType = "CFR"
            };

            var shipData = (from ship in db.ShipsRegister
                            where ship.Id == request.Item2.Ship.ShipId.Value
                            select new
                            {
                                ship.Cfr,
                                ship.Name
                            }).First();

            entry.ShipIdentifier = shipData.Cfr;
            entry.ShipName = shipData.Name;

            db.Fluxflaprequests.Add(entry);
            db.SaveChanges();

            return result;
        }

        private Task<bool> ProcessFlapResponse(FLUXFLAPResponseMessageType response, CancellationToken token)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IFluxPermitsDomainService service = serviceProvider.GetRequiredService<IFluxPermitsDomainService>();

            try
            {
                AddResponse(response.FLUXResponseDocument, response, serviceProvider);
                service.ReceiveFlapResponse(response);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                AddRequestError(response.FLUXResponseDocument, ex.Message, serviceProvider);
                LogException(ex, serviceProvider);
                return Task.FromResult(false);
            }
        }

        private Task<bool> ProcessFlapRequest(FLUXFLAPRequestMessageType request, CancellationToken token)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IFluxPermitsDomainService service = serviceProvider.GetRequiredService<IFluxPermitsDomainService>();

            try
            {
                AddOrUpdateRequest(request.FLUXReportDocument.ID[0],
                                   request,
                                   false,
                                   nameof(FluxFvmsDomainsEnum.FLAPDomain),
                                   nameof(ProcessFlapRequest),
                                   serviceProvider);
                service.ReceiveFlapRequest(request);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                AddRequestError(request.FLUXReportDocument.ID[0], ex.Message, serviceProvider);
                LogException(ex, serviceProvider);
                return Task.FromResult(false);
            }
        }
    }
}
