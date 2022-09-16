using IARA.Mobile.Application.DTObjects.Common;
using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionAquacultureDto : InspectionEditDto
    {
        public int? AquacultureId { get; set; }
        public string RepresentativeComment { get; set; }
        public string OtherFishingGear { get; set; }
        public LocationDto Location { get; set; }
        public List<InspectionCatchMeasureDto> CatchMeasures { get; set; }
    }
}
