using System;

namespace IARA.Mobile.Insp.Application.Filters
{
    public class InspectionsFilters
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public DateTime? UpdatedAfter { get; set; }
        public bool ShowInactiveRecords { get; set; }
    }
}
