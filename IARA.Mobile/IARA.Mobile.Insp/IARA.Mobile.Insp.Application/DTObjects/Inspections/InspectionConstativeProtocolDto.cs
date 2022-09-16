using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionConstativeProtocolDto : InspectionEditDto
    {
        public string InspectedPersonName { get; set; }
        public string InspectedObjectName { get; set; }
        public string Witness2Name { get; set; }
        public string Witness1Name { get; set; }
        public string InspectorName { get; set; }
        public string Location { get; set; }
        public List<InspectedCPFishingGearDto> FishingGears { get; set; }
        public List<InspectedCPCatchDto> Catches { get; set; }
    }
}
