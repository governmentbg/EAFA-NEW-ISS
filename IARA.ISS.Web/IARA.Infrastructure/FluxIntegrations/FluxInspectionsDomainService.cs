using System.Threading.Tasks;
using IARA.Flux.Models;
using IARA.Interfaces.Flux.InspectionAndSurveillance;
using TL.SysToSysSecCom;
using TLTTS.Common.ConfigModels;

namespace IARA.Infrastructure.FluxIntegration
{
    /// <summary>
    /// Inspection and Surveillance Domain
    /// </summary>
    public class FluxInspectionsDomainService : IFluxInspectionsDomainInitiatorService, IFluxInspectionsDomainReceiverService
    {
        private ISecureHttpClient secureHttpClient;
        //private IAsyncWorkerTaskQueue<FLUXSalesQueryMessageType, FluxNodeReponse> salesQueriesQueue;

        public FluxInspectionsDomainService(ISecureHttpClient secureHttpClient, ConnectionStrings connection)
        {
            this.secureHttpClient = secureHttpClient;
            //this.salesQueriesQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXSalesQueryMessageType, FluxNodeReponse>(ProcessFluxSalesQuery, connection, nameof(salesQueriesQueue));
        }

        public Task<bool> ReceiveResponse(FLUXResponseMessageType response)
        {
            throw new System.NotImplementedException();
        }

        public bool SendInspectionsReport(FLUXFLAPQueryMessageType report)
        {
            return false;
        }

    }
}
