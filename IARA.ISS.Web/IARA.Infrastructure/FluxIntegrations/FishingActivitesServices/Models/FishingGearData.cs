namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    internal class FishingGearData
    {
        public int FishingGearId { get; set; }

        public string FishingGearCode { get; set; }

        public int FishingGearRegisterId { get; set; }

        public bool HasHooks { get; set; }

        public int? HooksCount { get; set; }

        public decimal? NetEyeSize { get; set; }

        public decimal? LengthOrWidth { get; set; }

        public decimal? Height { get; set; }

        public int? LineCount { get; set; }

        public decimal? NetNominalLength { get; set; }

        public decimal? NumberNetsInFleet { get; set; }

        public string TrawlModel { get; set; }

        public int? CountOnBoard { get; set; }
    }
}
