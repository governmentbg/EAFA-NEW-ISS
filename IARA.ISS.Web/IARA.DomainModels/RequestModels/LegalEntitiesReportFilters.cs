namespace IARA.DomainModels.RequestModels
{
    public class LegalEntitiesReportFilters : BaseRequestModel
    {
        public string LegalName { get; set; }

        public string Eik { get; set; }

        public int? CountryId { get; set; }

        public int? PopulatedAreaId { get; set; }
    }
}
