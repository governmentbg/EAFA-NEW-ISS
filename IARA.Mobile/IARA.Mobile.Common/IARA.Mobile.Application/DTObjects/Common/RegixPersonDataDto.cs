using System;
using IARA.Mobile.Application.DTObjects.Common.API;

namespace IARA.Mobile.Application.DTObjects.Common
{
    public class RegixPersonDataDto
    {
        public EgnLncDto EgnLnc { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public PersonDocumentApiDto Document { get; set; }
        public int? CitizenshipCountryId { get; set; }
        public string CitizenshipCountryName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public bool? HasBulgarianAddressRegistration { get; set; }
        public int? GenderId { get; set; }
        public string GenderName { get; set; }
    }
}
