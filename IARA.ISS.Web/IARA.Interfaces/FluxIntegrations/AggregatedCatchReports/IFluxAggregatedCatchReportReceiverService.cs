using System.Threading.Tasks;
using IARA.Flux.Models;

namespace IARA.Interfaces.Flux.AggregatedCatchReports
{
    public interface IFluxAggregatedCatchReportReceiverService
    {
        Task<bool> ReportAggregatedCatches(FLUXACDRMessageType reportDocument);
    }
}
