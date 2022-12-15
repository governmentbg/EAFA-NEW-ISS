using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class PrintConfigurationFilters : BaseRequestModel
    {
        public List<int> TerritoryUnitIds { get; set; }

        public string UserEgnLnch { get; set; }

        public string UserNames { get; set; }

        public List<int> ApplicationTypeIds { get; set; }

        public string SubstituteReason { get; set; }
    }
}
