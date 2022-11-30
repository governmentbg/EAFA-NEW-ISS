using System.Collections.Generic;
using IARA.FVMSModels.FA;

namespace IARA.Interfaces.FVMSIntegrations
{
    public interface IFishingActivitiesService
    {
        List<OpenedLogBookPage> GetOpenedLogBookPages(string tripId);
    }
}
