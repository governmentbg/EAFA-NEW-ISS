namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    public class TripCatchRecordFishData
    {
        public int Id { get; set; }

        public decimal Quantity { get; set; }

        public int ShipLogBookPageId { get; set; }

        public bool IsLogBookPagePrimaryForTrip { get; set; }

        public string CatchCode { get; set; }

        public string CatchSizeClass { get; set; }
    }
}
