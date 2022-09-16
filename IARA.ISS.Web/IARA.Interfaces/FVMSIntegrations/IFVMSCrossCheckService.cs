using IARA.FVMSModels.CrossChecks;
using IARA.FVMSModels.GeoZones;

namespace IARA.Interfaces.FVMSIntegrations
{
    public interface IFVMSCrossCheckService
    {
        CCheckReport ReceiveQuery(CCheckQuery query);
        void ReceiveReport(CCheckReport report);
    }
}
