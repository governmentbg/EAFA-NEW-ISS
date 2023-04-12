using IARA.Infrastructure.FluxIntegrations.Helpers;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    internal class AddOriginDeclarationFishesNoCatchRecordParameters
    {
        public string SpeciesCode { get; set; }

        public HashSet<int> LogBookPageIds { get; set; }

        public FishingGearData FishingGearData { get; set; }

        public IDictionary<int, List<OriginDeclarationFish>> LogBookPageOriginDeclarationFishes { get; set; }

        public int? ZoneId { get; set; }

        public OriginDeclarationFishData OriginDeclarationFishData { get; set; }
    }
}
