using IARA.Mobile.Insp.Domain.Enums;
using System;
using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections.API
{
    public class InspectionApiDto
    {
        public int Id { get; set; }
        public string ReportNumber { get; set; }
        public DateTime StartDate { get; set; }
        public InspectionType InspectionType { get; set; }
        public InspectionState InspectionState { get; set; }
        public string Inspectors { get; set; }
        public List<int> InspectorsIds { get; set; }
        public string InspectionSubjects { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public bool CreatedByCurrentUser { get; set; } = true;
        public bool IsActive { get; set; }
    }
}
