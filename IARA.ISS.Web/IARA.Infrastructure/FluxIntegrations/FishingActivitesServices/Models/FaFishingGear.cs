namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    public class FaFishingGear : FaFishingGearCharacteristics
    {
        public int FishingGearRegisterId { get; set; }

        public int LogBookId { get; set; }

        public int LogBookPermitLicenseId { get; set; }
    }
}
