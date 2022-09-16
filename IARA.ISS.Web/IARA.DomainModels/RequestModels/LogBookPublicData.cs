using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class LogBookPublicData
    {
        public CatchesAndSalesPublicFilters Filters { get; set; }

        public List<int> LogBookIds { get; set; }
    }
}
