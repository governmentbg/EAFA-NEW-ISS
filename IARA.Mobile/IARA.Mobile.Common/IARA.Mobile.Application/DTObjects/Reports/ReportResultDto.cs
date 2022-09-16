using System.Collections.Generic;

namespace IARA.Mobile.Application.DTObjects.Reports
{
    public class ReportResultDto
    {
        public List<Dictionary<string, object>> Records { get; set; }
        public long TotalRecordsCount { get; set; }
    }
}
