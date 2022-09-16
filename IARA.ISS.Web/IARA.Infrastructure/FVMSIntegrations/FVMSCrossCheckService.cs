using IARA.DataAccess;
using IARA.FVMSModels.CrossChecks;
using IARA.FVMSModels.GeoZones;
using IARA.Interfaces.FVMSIntegrations;

namespace IARA.Infrastructure.FVMSIntegrations
{
    public class FVMSCrossCheckService : BaseService, IFVMSCrossCheckService
    {
        public FVMSCrossCheckService(IARADbContext dbContext)
            : base(dbContext)
        { }

        public void ReceiveReport(CCheckReport report)
        {

        }


        public CCheckReport ReceiveQuery(CCheckQuery query)
        {
            return null;
        }
    }
}
