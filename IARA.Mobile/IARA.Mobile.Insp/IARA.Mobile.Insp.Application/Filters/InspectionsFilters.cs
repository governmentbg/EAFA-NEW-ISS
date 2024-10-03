using System;
using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.Filters
{
    public class InspectionsFilters
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? UpdatedAfter { get; set; }
        public List<int> StateIds { get; set; }
        public string ReportNumber { get; set; }
        public int? InspectorId { get; set; }

        public bool ShowInactiveRecords { get; set; }
    }
}
