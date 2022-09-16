using System;

namespace IARA.DomainModels.RequestModels
{
    public class InspectionsFilters : BaseRequestModel
    {
        public int? ShipId { get; set; }
        public int? TerritoryNode { get; set; }
        public string Inspector { get; set; }
        public int? InspectionTypeId { get; set; }
        public string ReportNumber { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool? SubjectIsLegal { get; set; }
        public string SubjectName { get; set; }
        public string SubjectEIK { get; set; }
        public string SubjectEGN { get; set; }

        public DateTime? UpdatedAfter { get; set; }
        public bool? ShowBothActiveAndInactive { get; set; }
    }
}
