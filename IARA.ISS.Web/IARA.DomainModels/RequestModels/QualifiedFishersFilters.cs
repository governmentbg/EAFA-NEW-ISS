using System;

namespace IARA.DomainModels.RequestModels
{
    public class QualifiedFishersFilters : BaseRequestModel
    {
        public string Name { get; set; }
        public string EGN { get; set; }
        public DateTime? RegisteredDateFrom { get; set; }
        public DateTime? RegisteredDateTo { get; set; }
        public string RegistrationNum { get; set; }
        public string DiplomaNr { get; set; }
        public int? PersonId { get; set; }
        public int? TerritoryUnitId { get; set; }
    }
}
