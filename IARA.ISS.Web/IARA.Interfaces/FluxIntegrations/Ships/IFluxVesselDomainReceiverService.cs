using System.Threading.Tasks;
using IARA.Flux.Models;

namespace IARA.Interfaces.Flux
{
    public interface IFluxVesselDomainReceiverService
    {
        Task<bool> ReportVesselChange(FLUXReportVesselInformationType reportDocument);
    }
}
