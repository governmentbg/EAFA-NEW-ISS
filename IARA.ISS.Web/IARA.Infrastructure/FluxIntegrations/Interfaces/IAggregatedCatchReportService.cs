using System.Threading.Tasks;

namespace IARA.Infrastructure.FluxIntegrations.Interfaces
{
    public interface IAggregatedCatchReportService
    {
        /// <summary>
        /// Reports Aggregated catches for previous month
        /// </summary>
        /// <returns></returns>
        Task<bool> ReportAggregatedCatches();
    }
}
