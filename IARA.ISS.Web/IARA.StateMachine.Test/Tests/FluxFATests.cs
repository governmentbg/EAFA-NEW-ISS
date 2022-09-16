using System;
using System.Linq;
using System.Threading.Tasks;
using IARA.DataAccess;
using IARA.Flux.Models;
using IARA.Interfaces.Flux;
using Microsoft.Extensions.DependencyInjection;
using TL.SysToSysSecCom;

namespace IARA.Tests.Tests
{
    internal class FluxFATests : BaseTests
    {
        public FluxFATests(IServiceProvider provider)
            : base(provider)
        { }


        public void TestReporting()
        {
            int pastDays = 14;

            IFluxFishingActivitiesReceiverService fluxService = provider.GetService<IFluxFishingActivitiesReceiverService>();
            ISecureHttpClient client = provider.GetService<ISecureHttpClient>();
            IARADbContext db = provider.GetService<IARADbContext>();

            var fishingReports = db.Fluxfvmsrequests.Where(x => x.IsActive
                                      && x.WebServiceName == "ReceiveFishingActivitiesReport"
                                      && x.RequestDateTime >= DateTime.Now.AddDays(-pastDays))
                .OrderBy(x => x.RequestDateTime)
                .Select(x => x.RequestContent)
                .ToList();

            foreach (var content in fishingReports)
            {
                var message = TL.SysToSysSecCom.CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(content);
                fluxService.ReportFishingActivities(message);
            }

        }
    }
}
