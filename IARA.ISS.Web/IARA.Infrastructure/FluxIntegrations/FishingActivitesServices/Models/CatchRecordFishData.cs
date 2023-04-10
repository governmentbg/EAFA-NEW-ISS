namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    internal class CatchRecordFishData
    {
        public int LogBookPageId { get; set; }

        public int CatchRecordId { get; set; }

        public int? CatchRecordFishId { get; set; }

        public int FishId { get; set; }

        public string SpeciesCode { get; set; }

        public decimal Quantity { get; set; }

        public decimal UnloadedQuantity { get; set; }

        public int CatchZoneId { get; set; }

        public string faCatchZoneCode { get; set; }

        public int? OriginDeclarationId { get; set; }

        public int CatchSizeId { get; set; }

        public string FaCatchSizeTypeCode { get; set; }

        public int FishingGearRegisterId { get; set; }
    }
}
