using System;

namespace IARA.DomainModels.RequestModels
{
    public class SystemLogFilters : BaseRequestModel
    {
        public int? ActionTypeId { get; set; }
        public DateTime? RegisteredDateFrom { get; set; }
        public DateTime? RegisteredDateTo { get; set; }
        public int? UserId { get; set; }
        public string TableId { get; set; }
        public string TableName { get; set; }
    }
}
