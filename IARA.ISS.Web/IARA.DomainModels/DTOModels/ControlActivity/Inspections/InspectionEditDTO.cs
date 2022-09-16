using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionEditDTO
    {
        public int? Id { get; set; }

        public InspectionStatesEnum InspectionState { get; set; }

        public string ReportNum { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public InspectionTypesEnum? InspectionType { get; set; }

        public bool? ByEmergencySignal { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string InspectorComment { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? AdministrativeViolation { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ActionsTaken { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<InspectorDuringInspectionDTO> Inspectors { get; set; }

        [XmlIgnore]
        public List<FileInfoDTO> Files { get; set; }

        public List<InspectionSubjectPersonnelDTO> Personnel { get; set; }

        public List<InspectionCheckDTO> Checks { get; set; }

        public List<VesselDuringInspectionDTO> PatrolVehicles { get; set; }

        public List<InspectionObservationTextDTO> ObservationTexts { get; set; }

        public bool IsActive { get; set; }
    }
}
