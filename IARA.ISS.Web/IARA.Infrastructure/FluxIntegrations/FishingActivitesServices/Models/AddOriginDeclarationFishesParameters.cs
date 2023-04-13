using IARA.Infrastructure.FluxIntegrations.Helpers;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    internal class AddOriginDeclarationFishesParameters
    {
        public IDictionary<int, List<OriginDeclarationFish>> LogBookPageOriginDeclarationFishes { get; set; }

        public HashSet<int> LogBookPageIds { get; set; }

        public FishingGearData FishingGearData { get; set; }

        public List<CatchRecordFishData> CatchRecordFishesForUnloading { get; set; }

        public OriginDeclarationFishData OriginDeclarationFishData { get; set; }
    }
}
