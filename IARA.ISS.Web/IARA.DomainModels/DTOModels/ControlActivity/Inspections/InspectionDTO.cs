using System;
using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionDTO
    {
        public int Id { get; set; }
        public string ReportNumber { get; set; }
        public DateTime StartDate { get; set; }
        public InspectionTypesEnum InspectionType { get; set; }
        public string InspectionTypeName { get; set; }
        public InspectionStatesEnum InspectionState { get; set; }
        public string InspectionStateName { get; set; }
        public string Inspectors { get; set; }
        public string InspectionSubjects { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public List<AuanRegisterDTO> AUANs { get; set; }
        public bool IsActive { get; set; }
    }
}
