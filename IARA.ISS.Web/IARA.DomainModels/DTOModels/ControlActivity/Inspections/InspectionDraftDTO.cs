using System;
using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionDraftDTO
    {
        public int? Id { get; set; }
        public InspectionTypesEnum InspectionType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? ByEmergencySignal { get; set; }
        public string InspectorComment { get; set; }
        public bool? AdministrativeViolation { get; set; }
        public string ActionsTaken { get; set; }
        public string Json { get; set; }
        public List<FileInfoDTO> Files { get; set; }
    }
}
