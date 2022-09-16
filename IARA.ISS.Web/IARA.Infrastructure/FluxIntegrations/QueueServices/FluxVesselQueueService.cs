using System;
using System.Threading;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.Flux.Models;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Infrastructure.FluxIntegrations.QueueServices;
using IARA.Interfaces.Flux;
using TL.BatchWorkers.Interfaces;
using TLTTS.Common.ConfigModels;

namespace IARA.Infrastructure.FluxIntegration
{
    public class FluxVesselQueueService : BaseFluxService, IFluxVesselDomainReceiverService, IFluxVesselDomainInitiatorService
    {
        private readonly IAsyncWorkerTaskQueue<FLUXReportVesselInformationType, bool> vesselReportsQueue;
        private readonly IAsyncWorkerTaskQueue<FLUXVesselQueryMessageType, bool> vesselsQueriesQueue;
        private readonly IAsyncWorkerTaskQueue<FLUXReportVesselInformationType, bool> receiveVesselsQueue;

        public FluxVesselQueueService(ScopedServiceProviderFactory scopedServiceProviderFactory, ConnectionStrings connection)
           : base(scopedServiceProviderFactory, connection, nameof(responseQueue))
        {
            this.vesselsQueriesQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXVesselQueryMessageType, bool>(ProcessVesselQuery, connection, nameof(vesselsQueriesQueue));
            this.vesselReportsQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXReportVesselInformationType, bool>(ReportVesselChangeWorker, connection, nameof(vesselReportsQueue));
            this.receiveVesselsQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXReportVesselInformationType, bool>(ProcessReceivedVessel, connection, nameof(receiveVesselsQueue));
        }

        /// <summary>
        /// Заявка за данни за характеристики на риболовен кораб от централен Flux възел към ИСС
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<bool> ReceiveVesselQuery(FLUXVesselQueryMessageType query)
        {
            return vesselsQueriesQueue.Enqueue(query);
        }

        /// <summary>
        /// Рапортуване от ИСС на данни за характеристики или промяна им на риболовен кораб
        /// </summary>
        /// <param name="reportDocument"></param>
        /// <returns></returns>
        public Task<bool> ReportVesselChange(FLUXReportVesselInformationType reportDocument)
        {
            return this.vesselReportsQueue.Enqueue(reportDocument);
        }

        /// <summary>
        /// Актуализиране на статус в ИСС на изпратени данни към FLUX възел при изпращане от FLUX възел към централен FLUX възел
        /// Актуализиране на статус в ИСС на изпратена заявка за данни към FLUX възел при изпращане от FLUX възел към централен FLUX възел
        /// </summary>
        /// <param name="response"></param>
        public override Task<bool> ReceiveResponse(FLUXResponseMessageType response)
        {
            return base.ReceiveResponse(response);
        }

        /// <summary>
        /// Отговор на централен FLUX възел/ локален FLUX възел към ИСС
        /// Отговор на централен FLUX възел/ локален FLUX възел към ИСС на подадената заявка за данни
        /// </summary>
        /// <param name="response"></param>
        public Task<bool> VesselQueryReply(FLUXReportVesselInformationType response)
        {
            return receiveVesselsQueue.Enqueue(response);
        }

        /// <summary>
        /// Рапортуване на изисиканите данни за риболовен кораб
        /// </summary>
        /// <param name="query"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private Task<bool> ProcessVesselQuery(FLUXVesselQueryMessageType query, CancellationToken token)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IVesselsDomainService service = serviceProvider.GetRequiredService<IVesselsDomainService>();

            try
            {
                AddOrUpdateRequest(query.VesselQuery.ID, 
                                   query, 
                                   false, 
                                   nameof(FluxFvmsDomainsEnum.VesselDomain), 
                                   nameof(ProcessVesselQuery), 
                                   serviceProvider);
                FLUXReportVesselInformationType document = service.FindVessel(query);
                return ReportVesselChange(document);
            }
            catch (Exception ex)
            {
                AddRequestError(query.VesselQuery.ID, ex.Message, serviceProvider);
                LogException(ex, serviceProvider);
                return Task.FromResult(false);
            }
        }

        private Task<bool> ProcessReceivedVessel(FLUXReportVesselInformationType response, CancellationToken token)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IVesselsDomainService service = serviceProvider.GetRequiredService<IVesselsDomainService>();

            try
            {
                AddResponse(response.FLUXReportDocument, response, serviceProvider);
                service.ProcessVessel(response);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                AddRequestError(response.FLUXReportDocument, ex.Message, serviceProvider);
                LogException(ex, serviceProvider);
                return Task.FromResult(false);
            }
        }

        private Task<bool> ReportVesselChangeWorker(FLUXReportVesselInformationType reportDocument, CancellationToken token)
        {
            return Retry(() => SendMessageToFlux(reportDocument.FLUXReportDocument, 
                                                 reportDocument, 
                                                 nameof(FluxFvmsDomainsEnum.VesselDomain),
                                                 FluxEndpoints.FLUX_VESSEL_REPORT_DOCUMENT));
        }
    }
}
