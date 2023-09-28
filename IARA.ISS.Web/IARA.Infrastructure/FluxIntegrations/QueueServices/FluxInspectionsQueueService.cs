using System.Threading;
using System.Threading.Tasks;
using IARA.FluxInspectionModels;
using IARA.Infrastructure.FluxIntegration;
using IARA.Interfaces.Flux.InspectionAndSurveillance;
using TL.BatchWorkers.Abstractions.Interfaces.Queue;
using TL.Common.Settings;
using TL.Dependency.Abstractions;

namespace IARA.Infrastructure.FluxIntegrations.QueueServices
{
    public class FluxInspectionsQueueService : BaseFluxService, IFluxInspectionsDomainReceiverService, IFluxInspectionsDomainInitiatorService
    {
        private readonly IAsyncWorkerTaskQueue<FLUXISRMessageType, bool> reportsQueue;

        public FluxInspectionsQueueService(IScopedServiceProviderFactory scopedServiceProviderFactory, ConnectionStrings connection)
            : base(scopedServiceProviderFactory, connection, nameof(responseQueue))
        {
            this.reportsQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXISRMessageType, bool>(ReportInspectionWorker, connection, nameof(reportsQueue));
        }

        public Task<bool> ReportInspection(FLUXISRMessageType inspection)
        {
            return reportsQueue.Enqueue(inspection);
        }

        private Task<bool> ReportInspectionWorker(FLUXISRMessageType inspection, CancellationToken token)
        {
            return Retry(() => SendMessageToFlux(inspection.FLUXReportDocument,
                                                 inspection,
                                                 nameof(FluxFvmsDomainsEnum.InspectionsDomain),
                                                 FluxEndpoints.FLUX_INSPECTION_REPORT_DOCUMENT));
        }
    }
}
