using System.Threading.Tasks;
using IARA.Flux.Models;

namespace IARA.Infrastructure.FluxIntegrations.Interfaces
{
    public interface IVesselsDomainService
    {
        FLUXReportVesselInformationType FindVessel(FLUXVesselQueryMessageType query);
        void ProcessVessel(FLUXReportVesselInformationType vessel);
        Task<bool> ReportVesselChange(FLUXReportVesselInformationType vessel);
    }
}
