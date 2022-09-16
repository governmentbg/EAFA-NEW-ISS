namespace IARA.DomainModels.RequestModels
{
    public class FishingCapacityFilters : BaseRequestModel
    {
        public string ShipCfr { get; set; }

        public string ShipName { get; set; }

        public decimal? GrossTonnageFrom { get; set; }

        public decimal? GrossTonnageTo { get; set; }

        public decimal? PowerFrom { get; set; }

        public decimal? PowerTo { get; set; }

        public int? TerritoryUnitId { get; set; }
    }
}
