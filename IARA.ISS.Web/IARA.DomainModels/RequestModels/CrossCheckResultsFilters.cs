using System;
using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class CrossCheckResultsFilters : BaseRequestModel
    {
        public string CheckCode { get; set; }

        public string CheckName { get; set; }

        public string CheckTableName { get; set; }

        public string TableId { get; set; }

        public string ErrorDescription { get; set; }

        public List<int> ResolutionIds { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public int? AssignedUserId { get; set; }
    }
}
