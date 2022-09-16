namespace IARA.DomainModels.RequestModels
{
    public class ShipQuotasFilters : BaseRequestModel
    {
        public int? ShipId { get; set; }
        public int? Year { get; set; }
        public int? FishId { get; set; }
        public string Association { get; set; }
        public string CFR { get; set; }
    }
}
