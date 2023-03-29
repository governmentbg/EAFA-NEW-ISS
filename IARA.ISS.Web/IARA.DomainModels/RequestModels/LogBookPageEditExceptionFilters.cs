using System;
using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class LogBookPageEditExceptionFilters : BaseRequestModel
    {
        public int? UserId { get; set; }

        public List<int> LogBookTypeIds { get; set; }

        public int? LogBookId { get; set; }

        public DateTime? ExceptionActiveDateFrom { get; set; }

        public DateTime? ExceptionActiveDateTo { get; set; }

        public DateTime? EditPageDateFrom { get; set; }

        public DateTime? EditPageDateTo { get; set; }
    }
}
