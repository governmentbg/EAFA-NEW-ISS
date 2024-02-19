using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.RegixAbstractions.Enums;

namespace IARA.RegixIntegration.Models
{
    public class ApplicationContext
    {
        public int Id { get; set; }
        public int HistoryId { get; set; }
        public ApplicationHierarchyTypesEnum HierarchyType { get; set; }
        public List<BaseApplicationCheck> Checks { get; set; }

        public ApplicationContext(IEnumerable<ApplicationCheck> check)
        {
            this.Id = check.First().ApplicationId;
            this.HistoryId = check.First().ApplicationHistoryId;
            this.Checks = check.Select(x => new BaseApplicationCheck
            {
                CheckStatus = x.CheckStatus,
                ErrorDescription = x.ErrorDescription,
                ResponseStatus = x.ResponseStatus
            }).ToList();
        }

        public bool IsResponseMissing()
        {
            return Checks.Any(x => x.ResponseStatus == RegixResponseStatusEnum.NoResponse);
        }

        public bool IsFailed()
        {
            return Checks.Any(x => x.ResponseStatus == RegixResponseStatusEnum.Error || x.CheckStatus == RegixCheckStatusesEnum.ERROR);
        }

        public string GetErrorDescription()
        {
            return string.Join(';', this.Checks.Where(x => !string.IsNullOrEmpty(x.ErrorDescription)).Select(x => x.ErrorDescription));
        }

    }
}
