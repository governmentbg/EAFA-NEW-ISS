using System;
using IARA.Flux.Models;

namespace IARA.Interfaces.FluxIntegrations.AggregatedCatchReports
{
    public interface IAggregatedCatchesReportMapper
    {
        FLUXACDRMessageType GetAggragatedCatchesForPeriod(DateTime fromDate, DateTime toDate, Guid? referenceGuid = null);
    }
}
