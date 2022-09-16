namespace IARA.DomainModels.RequestModels
{
    public class PersonsReportFilters : BaseRequestModel
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string EGN { get; set; }

        public int? CountryId { get; set; }

        public int? PopulatedAreaId { get; set; }
    }
}
