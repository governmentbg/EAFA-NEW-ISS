using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionCheckDto
    {
        public int? Id { get; set; }
        public int CheckTypeId { get; set; }
        public CheckTypeEnum CheckValue { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
    }
}
