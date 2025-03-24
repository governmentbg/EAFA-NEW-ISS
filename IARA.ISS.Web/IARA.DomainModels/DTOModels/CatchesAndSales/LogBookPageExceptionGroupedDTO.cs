using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class LogBookPageExceptionGroupedDTO
    {
        public List<int> LogBookPageExceptionIds { get; set; }

        public string UserNames { get; set; }

        public string LogBookTypeNames { get; set; }

        public string LogBookNumber { get; set; }

        public bool IsValidNow { get; set; }

        public DateTime ExceptionActiveFrom { get; set; }

        public DateTime ExceptionActiveTo { get; set; }

        public DateTime EditPageFrom { get; set; }

        public DateTime EditPageTo { get; set; }

        public bool IsActive { get; set; }
    }
}
