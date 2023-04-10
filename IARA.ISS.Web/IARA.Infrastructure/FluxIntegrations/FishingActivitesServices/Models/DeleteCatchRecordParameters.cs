using IARA.Flux.Models;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    internal class DeleteCatchRecordParameters
    {
        public List<CatchRecord> RelatedDbCatches { get; set; }

        public List<CatchRecordFish> RelatedDbCatchFishes { get; set; }

        public FACatchType PrevFaCatch { get; set; }

        public HashSet<int> ShipLogBookPageIds { get; set; }

        public CatchRecord PrevCatchRecordData { get; set; }

        public FishingTripType SpecifiedFishingTrip { get; set; }
    }
}
