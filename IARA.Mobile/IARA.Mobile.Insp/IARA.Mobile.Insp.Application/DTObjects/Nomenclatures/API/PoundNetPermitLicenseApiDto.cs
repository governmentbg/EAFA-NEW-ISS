using System;
using IARA.Mobile.Domain.Interfaces;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API
{
    public class PoundNetPermitLicenseApiDto : IActive
    {
        public int Id { get; set; }
        public int PoundNetId { get; set; }
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
        public string PermitNumber { get; set; }
        public string LicenseNumber { get; set; }
        public int TypeId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
    }
}
