using System;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionPermitLicenseDTO
    {
        public int Id { get; set; }
        public string PermitNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string TypeName { get; set; }
        public int TypeId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
