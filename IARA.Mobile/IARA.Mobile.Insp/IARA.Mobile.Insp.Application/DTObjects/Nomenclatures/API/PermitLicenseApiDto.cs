using IARA.Mobile.Domain.Interfaces;
using System;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API
{
    public class PermitLicenseApiDto : IActive
    {
        public int Id { get; set; }
        public int ShipUid { get; set; }
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
        public int CaptainId { get; set; }
        public int PersonCaptainId { get; set; }
        public string PermitNumber { get; set; }
        public string LicenseNumber { get; set; }
        public int TypeId { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
    }
}
