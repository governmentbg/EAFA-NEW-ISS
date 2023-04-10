namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    internal class OriginDeclarationFishData
    {
        public int UnloadTypeId { get; set; }

        public int? FishPresentationId { get; set; }

        public int? FishFreshnessId { get; set; }

        public bool IsProcessedOnBoard { get; set; }

        public UnloadingTypesEnum UnloadingType { get; set; }

        public DateTime OperationDateTime { get; set; }

        public List<int> TranshipmenShipIds { get; set; }

        public int? PortId { get; set; }

        public decimal QuantityForProcessing { get; set; }
    }
}
