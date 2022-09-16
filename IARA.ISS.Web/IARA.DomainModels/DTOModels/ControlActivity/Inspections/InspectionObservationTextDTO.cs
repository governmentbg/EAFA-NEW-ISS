using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionObservationTextDTO
    {
        public int? Id { get; set; }
        public string Text { get; set; }
        public InspectionObservationCategoryEnum Category { get; set; }
    }
}
