using IARA.Mobile.Application.DTObjects.Common.API;
using IARA.Mobile.Domain.Enums;
using System.Collections.Generic;

namespace IARA.Mobile.Application.DTObjects.Profile.API
{
    public class BaseProfileApiDto : BasePersonInfoApiDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public List<AddressRegistrationApiDto> UserAddresses { get; set; }
        public List<RoleApiDto> Roles { get; set; }
        public List<RegixLegalDataApiDto> Legals { get; set; }
        public NewsSubscriptionType NewsSubscription { get; set; }
        public List<UserNewsDistrictSubscriptionDto> NewsDistrictSubscriptions { get; set; }
    }
}
