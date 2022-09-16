using IARA.Mobile.Application.DTObjects.Common.API;
using System;
using System.Collections.Generic;

namespace IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API
{
    public class PersonDto
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Egn { get; set; }
        public int CitizenshipId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public bool HasBulgarianAddressRegistration { get; set; }
        public bool PermanentAddressMatchWithCorrespondence { get; set; }
        public PersonDocumentApiDto Document { get; set; }
        public List<BaseAddressDto> Addresses { get; set; }
    }
}
