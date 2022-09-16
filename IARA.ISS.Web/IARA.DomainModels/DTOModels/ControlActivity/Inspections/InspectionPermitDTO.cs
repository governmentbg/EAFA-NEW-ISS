using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionPermitDTO
    {
        public int? Id { get; set; }
        public InspectionToggleTypesEnum? CheckValue { get; set; }
        public string Description { get; set; }
        public int? PermitLicenseId { get; set; }
        public string PermitNumber { get; set; }
        public string LicenseNumber { get; set; }
        public int? TypeId { get; set; }
        public string TypeName { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
