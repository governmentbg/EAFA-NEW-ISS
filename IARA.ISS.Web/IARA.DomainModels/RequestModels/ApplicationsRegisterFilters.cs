using System;

namespace IARA.DomainModels.RequestModels
{
    public class ApplicationsRegisterFilters : BaseRequestModel
    {
        public string AccessCode { get; set; }
        public string EventisNum { get; set; }
        public int? ApplicationTypeId { get; set; }
        public int? ApplicationStatusId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public string SubmittedFor { get; set; }
        public string SubmittedForEgnLnc { get; set; }
        public int? ApplicationSourceId { get; set; }
        public bool? ShowAssignedApplications { get; set; }
        public string AssignedTo { get; set; }
        public bool? ShowOnlyNotFinished { get; set; }
        public int? TerritoryUnitId { get; set; }
    }
}
