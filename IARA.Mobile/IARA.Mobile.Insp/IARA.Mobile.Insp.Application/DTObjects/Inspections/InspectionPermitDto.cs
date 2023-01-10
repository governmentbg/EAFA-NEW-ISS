using System;
using IARA.Mobile.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionPermitDto
    {
        public int? Id { get; set; }
        public CheckTypeEnum? CheckValue { get; set; }
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
