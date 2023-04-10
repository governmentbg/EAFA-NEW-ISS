using IARA.Infrastructure.FluxIntegrations.Helpers;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    internal class FishingOperationData
    {
        public FishingGearCatchRecordData FishingGearCatchRecord { get; set; }

        public IDictionary<int, List<CatchRecordFish>> FishingGearRegisterCatchRecordFishes { get; set; }
    }
}
