using System;

namespace IARA.DomainModels.RequestModels
{
    public class LegalEntitiesFilters : BaseRequestModel
    {
        public string LegalName { get; set; }

        public string Eik { get; set; }

        public DateTime? RegisteredDateFrom { get; set; }

        public DateTime? RegisteredDateTo { get; set; }

        public int? TerritoryUnitId { get; set; }
    }
}
