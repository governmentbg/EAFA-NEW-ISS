using System.Collections.Generic;
using IARA.Mobile.Application.DTObjects.Common;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionFisherDto : InspectionEditDto
    {
        public string TicketNum { get; set; }
        public int? FishingRodsCount { get; set; }
        public int? FishingHooksCount { get; set; }
        public string FishermanComment { get; set; }
        public string InspectionAddress { get; set; }
        public LocationDto InspectionLocation { get; set; }
        public List<InspectionCatchMeasureDto> CatchMeasures { get; set; }
    }
}
