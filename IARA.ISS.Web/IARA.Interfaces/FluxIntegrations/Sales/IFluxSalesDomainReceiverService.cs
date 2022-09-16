using System.Threading.Tasks;
using IARA.Flux.Models;

namespace IARA.Interfaces.Flux
{
    public interface IFluxSalesDomainReceiverService
    {
        Task<bool> CreateFluxSalesQuery(FLUXSalesQueryMessageType query);
        Task<bool> ReportSalesDocument(FLUXSalesReportMessageType message);
    }
}
