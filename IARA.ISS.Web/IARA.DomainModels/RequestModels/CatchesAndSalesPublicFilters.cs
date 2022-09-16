using System;
using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class CatchesAndSalesPublicFilters : BaseRequestModel
    {
        public decimal? PageNumber { get; set; }

        public string OnlinePageNumber { get; set; }

        public decimal? DocumentNumber { get; set; }

        public int? LogBookTypeId { get; set; }

        public string LogBookNumber { get; set; }

        public List<int> LogBookStatusIds { get; set; }

        public DateTime? LogBookValidityStartDate { get; set; }

        public DateTime? LogBookValidityEndDate { get; set; }
    }
}
