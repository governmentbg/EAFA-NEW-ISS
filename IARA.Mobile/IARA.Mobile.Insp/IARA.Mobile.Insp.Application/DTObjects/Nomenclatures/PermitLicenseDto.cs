using System;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures
{
    public class PermitLicenseDto
    {
        public int Id { get; set; }
        public string PermitNumber { get; set; }
        public string LicenseNumber { get; set; }
        public int TypeId { get; set; }
        public string TypeName { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
