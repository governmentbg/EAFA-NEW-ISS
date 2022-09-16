using System.Threading;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.Enums;
using IARA.Common.Utils;
using IARA.Flux.Models;
using IARA.Infrastructure.FluxIntegrations.QueueServices;
using IARA.Interfaces.Flux.AggregatedCatchReports;
using TL.BatchWorkers.Interfaces;
using TLTTS.Common.ConfigModels;

namespace IARA.Infrastructure.FluxIntegration
{
    public class ACDRQueueService : BaseFluxService, IFluxAggregatedCatchReportReceiverService, IFluxAggregatedCatchReportInitiatorService
    {
        private readonly IAsyncWorkerTaskQueue<FLUXACDRMessageType, bool> acdReportsQueue;

        public ACDRQueueService(ScopedServiceProviderFactory scopedServiceProviderFactory, ConnectionStrings connection)
            : base(scopedServiceProviderFactory, connection, nameof(responseQueue))
        {
            this.acdReportsQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXACDRMessageType, bool>(SendAggregatedCatchReport, 
                                                                                                    connection, 
                                                                                                    nameof(acdReportsQueue));
        }

        /// <summary>
        /// Рапортуване от ИСС на обобщени данни
        /// </summary>
        public Task<bool> ReportAggregatedCatches(FLUXACDRMessageType reportDocument)
        {
            return this.acdReportsQueue.Enqueue(reportDocument);
        }

        /// <summary>
        /// Актуализиране на статус в ИСС на изпратени данни към FLUX възел при изпращане от FLUX възел към централен FLUX възел
        /// Отговор на централен FLUX възел/ локален FLUX възел към ИСС
        /// </summary>
        /// <param name="response"></param>
        public override Task<bool> ReceiveResponse(FLUXResponseMessageType response)
        {
            return base.ReceiveResponse(response);
        }

        private Task<bool> SendAggregatedCatchReport(FLUXACDRMessageType reportDocument, CancellationToken token)
        {
            return Retry(() => SendMessageToFlux(reportDocument.FLUXReportDocument, 
                                                 reportDocument, 
                                                 nameof(FluxFvmsDomainsEnum.ACDRDomain), 
                                                 FluxEndpoints.FLUX_ACDR_REPORT_MESSAGE));
        }
    }
}
