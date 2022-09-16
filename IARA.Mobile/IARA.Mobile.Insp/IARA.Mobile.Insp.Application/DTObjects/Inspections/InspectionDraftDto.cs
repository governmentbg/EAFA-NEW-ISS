using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Domain.Enums;
using System;
using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionDraftDto
    {
        public int? Id { get; set; }
        public InspectionType InspectionType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? ByEmergencySignal { get; set; }
        public string InspectorComment { get; set; }
        public bool? AdministrativeViolation { get; set; }
        public string ActionsTaken { get; set; }
        public string Json { get; set; }
        public List<FileModel> Files { get; set; }
    }
}
