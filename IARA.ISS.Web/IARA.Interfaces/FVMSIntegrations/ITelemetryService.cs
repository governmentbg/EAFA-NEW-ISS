using System.Collections.Generic;
using IARA.FVMSModels;

namespace IARA.Interfaces.FVMSIntegrations
{
    public interface ITelemetryService
    {
        void WriteTelemetry(List<TelemetryStatus> telemetryData);
    }
}
