using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionCheckToolMarkDTO : InspectionEditDTO
    {
        public VesselDuringInspectionDTO InspectedShip { get; set; }
        public PortVisitDTO Port { get; set; }
        public int? PoundNetId { get; set; }
        public int? CheckReasonId { get; set; }
        public int? RecheckReasonId { get; set; }
        public string OtherRecheckReason { get; set; }

        public int? PermitId { get; set; }
        public List<InspectedFishingGearDTO> FishingGears { get; set; }
    }
}
