using IARA.Mobile.Application.DTObjects.Common.API;
using IARA.Mobile.Application.DTObjects.Profile.API;
using IARA.Mobile.Domain.Models;
using System.Collections.Generic;

namespace IARA.Mobile.Pub.Application.DTObjects.FishingTickets.API
{
    public class UpdateTicketProfileDataDto
    {
        public BasePersonInfoApiDto Person { get; set; }
        public FileModel Photo { get; set; }
        public List<AddressRegistrationApiDto> UserAddresses { get; set; }
    }
}
