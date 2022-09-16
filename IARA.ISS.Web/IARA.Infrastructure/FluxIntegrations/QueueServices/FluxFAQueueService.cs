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
    public class FluxFAQueueService : BaseFluxService, IFluxFishingActivitiesReceiverService, IFluxFishingActivitiesInitiatorService
    {
        private IAsyncWorkerTaskQueue<FLUXFAQueryMessageType, bool> fishingActivitiesQueriesQueue;
        private IAsyncWorkerTaskQueue<FLUXFAReportMessageType, bool> fishingActivitiesReportQueue;
        private IAsyncWorkerTaskQueue<FLUXFAReportMessageType, bool> receiveFishingActivitiesQueue;

        public FluxFAQueueService(ScopedServiceProviderFactory scopedServiceProviderFactory, ConnectionStrings connection)
            : base(scopedServiceProviderFactory, connection, nameof(responseQueue))
        {
            this.fishingActivitiesQueriesQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXFAQueryMessageType, bool>(ProcessFluxFAQuery, connection, nameof(fishingActivitiesQueriesQueue));
            this.fishingActivitiesReportQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXFAReportMessageType, bool>(SendFishingActivitiesReport, connection, nameof(fishingActivitiesReportQueue));
            this.receiveFishingActivitiesQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXFAReportMessageType, bool>(ReceiveFishingActivitiesReport, connection, nameof(receiveFishingActivitiesQueue));
        }

        /// <summary>
        /// Получаване на отговор от Централен Flux възел при рапортуване на риболовни активности
        /// Актуализиране на статус в ИСС на изпратени данни към FLUX възел при изпращане от FLUX възел към централен FLUX възел
        /// </summary>
        /// <param name="reponse"></param>
        public Task<bool> FAReportReceived(FLUXFAReportMessageType reponse)
        {
            return this.receiveFishingActivitiesQueue.Enqueue(reponse);
        }

        /// <summary>
        /// Получаване на заявка за данни от риболовни дейности
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<bool> ReceiveFAQuery(FLUXFAQueryMessageType query)
        {
            return fishingActivitiesQueriesQueue.Enqueue(query);
        }

        /// <summary>
        /// 1. Рапортуване на данни от риболовни дейности
        /// 2. Съставяне на отговор с данни от риболовни дейности
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public Task<bool> ReportFishingActivities(FLUXFAReportMessageType report)
        {
            return fishingActivitiesReportQueue.Enqueue(report);
        }

        public override Task<bool> ReceiveResponse(FLUXResponseMessageType response)
        {
            return responseQueue.Enqueue(response);
        }

        private Task<bool> ProcessFluxFAQuery(FLUXFAQueryMessageType query, CancellationToken token)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IFishingActivitiesDomainService service = serviceProvider.GetRequiredService<IFishingActivitiesDomainService>();

            try
            {
                AddOrUpdateRequest(query.FAQuery.ID, 
                                   query, 
                                   false, 
                                   nameof(FluxFvmsDomainsEnum.FADomain), 
                                   nameof(ProcessFluxFAQuery), 
                                   serviceProvider);
                FLUXFAReportMessageType document = service.ProcessFluxFAQuery(query);
                return ReportFishingActivities(document);
            }
            catch (Exception ex)
            {
                AddRequestError(query.FAQuery.ID, ex.Message, serviceProvider);
                LogException(ex, serviceProvider);
                return Task.FromResult(false);
            }
        }

        private Task<bool> ReceiveFishingActivitiesReport(FLUXFAReportMessageType report, CancellationToken token)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            IFishingActivitiesDomainService service = serviceProvider.GetRequiredService<IFishingActivitiesDomainService>();

            try
            {
                AddOrUpdateRequest(report.FLUXReportDocument, 
                                   report, 
                                   false, 
                                   nameof(FluxFvmsDomainsEnum.FADomain), 
                                   nameof(ReceiveFishingActivitiesReport), 
                                   serviceProvider);

                this.fishingActivitiesReportQueue.Enqueue(report);

                service.ReceiveFishingActivitiesReport(report);

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                AddRequestError(report.FLUXReportDocument, ex.Message, serviceProvider);
                LogException(ex, serviceProvider);
                return Task.FromResult(false);
            }
        }

        private Task<bool> SendFishingActivitiesReport(FLUXFAReportMessageType report, CancellationToken token)
        {
            return Retry(() => SendMessageToFlux(report.FLUXReportDocument, 
                                                 report, 
                                                 nameof(FluxFvmsDomainsEnum.FADomain), 
                                                 FluxEndpoints.FLUX_FA_REPORT));
        }
    }
}
