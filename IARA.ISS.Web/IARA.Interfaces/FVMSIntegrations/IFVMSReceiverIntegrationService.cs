using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IARA.FVMSModels;
using IARA.FVMSModels.ExternalModels;

namespace IARA.Interfaces.FVMSIntegrations
{
    public interface IFVMSReceiverIntegrationService
    {
        Task<bool> EnqueuePermitChange(Permit permit);
        Task<bool> EnqueuePermitsChange(List<Permit> permits);
        Task<List<TelemetryStatus>> GetTelemetries(List<TelemetryQuery> queries, CancellationToken? token = null);
        Task<List<TelemetryStatus>> GetVesselTelemetries(TelemetryQuery query, CancellationToken? token = null);
    }
}
