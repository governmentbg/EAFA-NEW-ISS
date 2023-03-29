namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    internal class FishingGearCatchRecordData
    {
        public int? FishingGearRegisterId { get; set; }

        public CatchRecord CatchRecord { get; set; }

        public string FishingTripIdentifier { get; set; }
    }
}
