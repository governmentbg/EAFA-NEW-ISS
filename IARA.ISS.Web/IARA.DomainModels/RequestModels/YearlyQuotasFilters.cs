using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.DomainModels.RequestModels
{
    public class YearlyQuotasFilters : BaseRequestModel
    {
        public int? Year { get; set; }
        public int? FishId { get; set; }
    }
}
