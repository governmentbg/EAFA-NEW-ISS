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
    public class FluxSalesQueueService : BaseFluxService, IFluxSalesDomainInitiatorService, IFluxSalesDomainReceiverService
    {
        private IAsyncWorkerTaskQueue<FLUXSalesReportMessageType, bool> saleReportsQueue;
        private IAsyncWorkerTaskQueue<FLUXSalesQueryMessageType, bool> receiveSalesQueryQueue;
        private readonly IAsyncWorkerTaskQueue<FLUXSalesQueryMessageType, bool> sendSalesQueryQueue;

        public FluxSalesQueueService(ScopedServiceProviderFactory scopedServiceProviderFactory, ConnectionStrings connection)
            : base(scopedServiceProviderFactory, connection, nameof(responseQueue))
        {
            this.receiveSalesQueryQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXSalesQueryMessageType, bool>(ProcessFluxSalesQuery, connection, nameof(receiveSalesQueryQueue));
            this.saleReportsQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXSalesReportMessageType, bool>(SendSalesReport, connection, nameof(saleReportsQueue));
            this.sendSalesQueryQueue = WorkerCreationUtils.CreateWorkerQueue<FLUXSalesQueryMessageType, bool>(SendSalesQuery, connection, nameof(sendSalesQueryQueue));
        }


        /// <summary>
        /// Изпращане на заявка за данни към централен Flux възел
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<bool> CreateFluxSalesQuery(FLUXSalesQueryMessageType query)
        {
            return sendSalesQueryQueue.Enqueue(query);
        }

        /// <summary>
        /// 1. Актуализиране на статус в ИСС на изпратени данни към FLUX възел при изпращане от FLUX възел към централен FLUX възел
        /// 2. Получаване на отговор от Централен Flux възел при рапортуване на данни за деклрации за първа продажба, приемане
        /// </summary>
        /// <param name="response"></param>
        public Task<bool> FluxSalesResponse(FLUXResponseMessageType response)
        {
            return responseQueue.Enqueue(response);
        }

        /// <summary>
        /// Получаване на заявка
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public Task<bool> ReceiveFluxSalesQuery(FLUXSalesQueryMessageType query)
        {
            return receiveSalesQueryQueue.Enqueue(query);
        }

        /// <summary>
        /// Получаване на отговор с данни от направена заявка
        /// Получаване на данни от декларации за първа продажба, приемане
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        public Task<bool> ReceiveSalesReport(FLUXSalesReportMessageType report)
        {
            return null;
        }

        /// <summary>
        /// Рапортуване на данни от декларации за първа продажба, приемане
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task<bool> ReportSalesDocument(FLUXSalesReportMessageType message)
        {
            return this.saleReportsQueue.Enqueue(message);
        }

        /// <summary>
        /// Съставяне на отговор с данни при получена заявка
        /// </summary>
        /// <param name="query"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private Task<bool> ProcessFluxSalesQuery(FLUXSalesQueryMessageType query, CancellationToken token)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            ISalesDomainService service = serviceProvider.GetRequiredService<ISalesDomainService>();

            try
            {
                AddOrUpdateRequest(query.SalesQuery.ID, 
                                   query, 
                                   false,
                                   nameof(FluxFvmsDomainsEnum.SalesDomain),
                                   nameof(ProcessFluxSalesQuery),
                                   serviceProvider);
                FLUXSalesReportMessageType document = service.FindSalesDocuments(query);
                return ReportSalesDocument(document);
            }
            catch (Exception ex)
            {
                AddRequestError(query.SalesQuery.ID, ex.Message, serviceProvider);
                LogException(ex, serviceProvider);
                return Task.FromResult(false);
            }
        }

        private Task<bool> SendSalesReport(FLUXSalesReportMessageType report, CancellationToken token)
        {
            return Retry(() => SendMessageToFlux(report.FLUXReportDocument, 
                                                 report, 
                                                 nameof(FluxFvmsDomainsEnum.SalesDomain), 
                                                 FluxEndpoints.FLUX_SALES_REPORT_MESSAGE));
        }

        private Task<bool> SendSalesQuery(FLUXSalesQueryMessageType query, CancellationToken token)
        {
            return Retry(() => SendMessageToFlux(query.SalesQuery.ID, 
                                                 query, 
                                                 nameof(FluxFvmsDomainsEnum.SalesDomain), 
                                                 FluxEndpoints.FLUX_SALES_QUERY));
        }

        /// <summary>
        /// Съставяне на отговор от ИСС при получаване на данни за деклaрации за първа продажба, приемане
        /// </summary>
        public Task<bool> CreateResponse(FLUXResponseMessageType response)
        {
            return Task.FromResult(true);
        }

    }
}
