using System;

namespace IARA.Mobile.Application.DTObjects.Common.API
{
    public class BasePersonInfoApiDto
    {
        public EgnLncDto EgnLnc { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public PersonDocumentApiDto Document { get; set; }
        public int? CitizenshipCountryId { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool? HasBulgarianAddressRegistration { get; set; }
        public int? GenderId { get; set; }
    }
}
