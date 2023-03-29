namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    internal class FishingGearCharacteristics
    {
        public int? GearCount { get; set; }

        public decimal? MeshSize { get; set; }

        public int? HooksCount { get; set; }

        public string PermitLicenseNumber { get; set; }

        public decimal LengthOrWidth { get; set; }

        public decimal Height { get; set; }

        public decimal NominalLength { get; set; }

        public int LinesCount { get; set; }

        public int NetsCount { get; set; }

        public string TrawlModel { get; set; }
    }
}
