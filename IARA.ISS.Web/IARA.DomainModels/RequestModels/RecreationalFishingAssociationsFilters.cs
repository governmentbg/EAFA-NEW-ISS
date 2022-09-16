namespace IARA.DomainModels.RequestModels
{
    public class RecreationalFishingAssociationsFilters : BaseRequestModel
    {
        public string Name { get; set; }
        public string EIK { get; set; }
        public int? TerritoryUnitId { get; set; }
        public int? RepresentativePersonId { get; set; }
        public bool? ShowCanceled { get; set; }
    }
}
