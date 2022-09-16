using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionConstativeProtocolDTO : InspectionEditDTO
    {
        public string InspectedPersonName { get; set; }
        public string InspectedObjectName { get; set; }
        public string Witness2Name { get; set; }
        public string Witness1Name { get; set; }
        public string InspectorName { get; set; }
        public string Location { get; set; }
        public List<InspectedCPFishingGearDTO> FishingGears { get; set; }
        public List<InspectedCPCatchDTO> Catches { get; set; }
    }
}
