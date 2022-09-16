using System;
using System.Threading.Tasks;
using IARA.DataAccess;
using IARA.Flux.Models;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Interfaces.Flux.AggregatedCatchReports;
using IARA.Interfaces.FluxIntegrations.AggregatedCatchReports;

namespace IARA.Infrastructure.FluxIntegrations
{
    public class AggregatedCatchReportService : BaseService, IAggregatedCatchReportService
    {
        private IAggregatedCatchesReportMapper mapper;
        private IFluxAggregatedCatchReportReceiverService service;

        public AggregatedCatchReportService(IARADbContext dbContext,
                                            IAggregatedCatchesReportMapper mapper,
                                            IFluxAggregatedCatchReportReceiverService service)
            : base(dbContext)
        {
            this.mapper = mapper;
            this.service = service;
        }

        public Task<bool> ReportAggregatedCatches()
        {
            int month = 1;
            DateTime previousMonth = DateTime.UtcNow.Date.AddMonths(-1 * month);
            DateTime startOfMonth = previousMonth.AddDays(-(previousMonth.Day - 1));
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            FLUXACDRMessageType aggregatedCatchReport = mapper.GetAggragatedCatchesForPeriod(startOfMonth, endOfMonth);
            return service.ReportAggregatedCatches(aggregatedCatchReport);
        }
    }
}
