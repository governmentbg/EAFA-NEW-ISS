using IARA.Mobile.Application.DTObjects.Common.API;
using IARA.Mobile.Application.DTObjects.Profile.API;
using IARA.Mobile.Domain.Models;
using System;
using System.Collections.Generic;

namespace IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API
{
    public class FishingTicketRequestDto
    {
        public int Id { get; set; }
        public int TypeId { get; set; }
        public int PeriodId { get; set; }
        public decimal Price { get; set; }
        public DateTime ValidFrom { get; set; }
        public FileModel PersonPhoto { get; set; }
        public BasePersonInfoApiDto Person { get; set; }
        public List<AddressRegistrationApiDto> PersonAddressRegistrations { get; set; }
        public BasePersonInfoApiDto RepresentativePerson { get; set; }
        public List<AddressRegistrationApiDto> RepresentativePersonAddressRegistrations { get; set; }
        public MembershipCardDto MembershipCard { get; set; }
        public int ApplicationId { get; set; }
        public TelkReviewDto TelkData { get; set; }
        public string StatusReason { get; set; }
        public bool HasUserConfirmed { get; set; }
        public List<FileModel> Files { get; set; }
        public bool UpdateProfileData { get; set; }
    }
}
