using System.Collections.Generic;
using IARA.DataAccess;
using IARA.FVMSModels;
using IARA.Interfaces.FVMSIntegrations;

namespace IARA.Infrastructure.FVMSIntegrations
{
    public class TelemetryService : BaseService, ITelemetryService
    {
        public TelemetryService(IARADbContext dbContext)
            : base(dbContext)
        { }

        public void WriteTelemetry(List<TelemetryStatus> telemetryData)
        {

        }
    }
}
