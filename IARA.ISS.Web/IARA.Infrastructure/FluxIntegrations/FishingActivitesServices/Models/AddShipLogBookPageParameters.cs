using IARA.Flux.Models;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    internal class AddShipLogBookPageParameters
    {
        public FishingActivityType FishingActivity { get; set; }

        public FishingGearType FishingGear { get; set; }
        
        public List<int> ShipIds { get; set; }
    }
}
