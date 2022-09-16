using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionCheckDTO
    {
        public int? Id { get; set; }
        public int? CheckTypeId { get; set; }
        public InspectionToggleTypesEnum CheckValue { get; set; }
        public string Description { get; set; }
        public string Number { get; set; }
    }
}
