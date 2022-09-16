using System;
using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class BuyersFilters : BaseRequestModel
    {
        public int? EntryTypeId { get; set; }
        public string UrorrNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public DateTime? RegisteredDateFrom { get; set; }
        public DateTime? RegisteredDateTo { get; set; }
        public string UtilityName { get; set; }
        public int? PopulatedAreaId { get; set; }
        public int? DistrictId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEIK { get; set; }
        public string OrganizerName { get; set; }
        public string OrganizerEGN { get; set; }
        public string LogBookNumber { get; set; }
        public int? TerritoryUnitId { get; set; }
        public List<int> StatusIds { get; set; }
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
    }
}
