using IARA.Common.Enums;
using IARA.RegixAbstractions.Enums;

namespace IARA.RegixIntegration.Models
{
    public class ApplicationCheck : BaseApplicationCheck
    {
        public int ApplicationId { get; set; }
        public int ApplicationHistoryId { get; set; }
        public ApplicationHierarchyTypesEnum ApplicationHierarchyType { get; set; }
    }

    public class BaseApplicationCheck
    {
        public RegixCheckStatusesEnum CheckStatus { get; set; }
        public RegixResponseStatusEnum ResponseStatus { get; set; }
        public string ErrorDescription { get; set; }
    }
}
