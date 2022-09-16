using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class LogBookAdministrationData
    {
        public CatchesAndSalesAdministrationFilters Filters { get; set; }

        public List<int> LogBookIds { get; set; }
    }
}
