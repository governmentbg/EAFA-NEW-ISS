using IARA.Common.Enums;

namespace IARA.RegixAbstractions.Models
{
    public class BaseContextData
    {
        public string ServiceURI { get; set; }
        public string ServiceType { get; set; }
        public string EmployeeIdentifier { get; set; }
        public string EGN { get; set; }
        public string EmployeeNames { get; set; }
        public int ApplicationId { get; set; }
        public int ApplicationHistoryId { get; set; }
        public string AdditionalIdentifier { get; set; }
        public ContextCheckType Type { get; set; }
    }
}
