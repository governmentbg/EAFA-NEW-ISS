using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class CrossChecksFilters : BaseRequestModel
    {
        public string Name { get; set; }

        public string CheckedTable { get; set; }

        public string DataSource { get; set; }

        public string ReportGroupName { get; set; }

        public List<short> ErrorLevels { get; set; }

        public List<string> AutoExecFrequencyCodes { get; set; }
    }
}
