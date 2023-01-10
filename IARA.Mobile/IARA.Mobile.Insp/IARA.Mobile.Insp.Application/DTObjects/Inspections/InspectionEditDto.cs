using System;
using System.Collections.Generic;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionEditDto
    {
        public int? Id { get; set; }
        public InspectionState InspectionState { get; set; }
        public string ReportNum { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public InspectionType InspectionType { get; set; }
        public bool? ByEmergencySignal { get; set; }
        public string InspectorComment { get; set; }
        public bool? AdministrativeViolation { get; set; }
        public string ActionsTaken { get; set; }
        public List<InspectorDuringInspectionDto> Inspectors { get; set; }
        public List<FileModel> Files { get; set; }
        public List<InspectionSubjectPersonnelDto> Personnel { get; set; }
        public List<InspectionCheckDto> Checks { get; set; }
        public List<VesselDuringInspectionDto> PatrolVehicles { get; set; }
        public List<InspectionObservationTextDto> ObservationTexts { get; set; }

        /// <summary>
        /// Passed in from the ViewModel to the transaction.
        /// Not used by the server.
        /// </summary>
        public string LocalIdentifier { get; set; }

        /// <summary>
        /// Passed in from the ViewModel to the transaction.
        /// Not used by the server.
        /// </summary>
        public bool IsOfflineOnly { get; set; }
    }
}
