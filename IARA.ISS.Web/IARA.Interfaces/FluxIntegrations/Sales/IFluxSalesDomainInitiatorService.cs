using System.Threading.Tasks;
using IARA.Flux.Models;
using IARA.Interfaces.FluxIntegrations;

namespace IARA.Interfaces.Flux
{
    public interface IFluxSalesDomainInitiatorService : IBaseFluxService
    {
        /// <summary>
        /// Получаване на данни от декларации за първа продажба, приемане
        /// Получаване на отговор с данни от направена заявка
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        Task<bool> ReceiveSalesReport(FLUXSalesReportMessageType report);

        /// <summary>
        /// Получаване на заявка
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        Task<bool> ReceiveFluxSalesQuery(FLUXSalesQueryMessageType query);
    }
}
