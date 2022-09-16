using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionObservationTextDto
    {
        public int? Id { get; set; }
        public string Text { get; set; }
        public InspectionObservationCategory Category { get; set; }
    }
}
