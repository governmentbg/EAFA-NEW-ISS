using IARA.Security.Enums;
using IARA.Security.Interfaces;
using IARA.Security.Models.SecurityModels;
using IARA.Security.Services;
using Microsoft.AspNetCore.Http;

namespace IARA.IdentityProfile.IdentityProfiles
{
    public class InspectorsMobileProfileClientService : BaseProfileClientService
    {
        public InspectorsMobileProfileClientService(PasswordHasher passwordHasher,
                                                    ISecurityService securityService,
                                                    SecurityEmailSender emailSenderQueue,
                                                    IHttpContextAccessor httpContext)
         : base(passwordHasher, securityService, emailSenderQueue, httpContext)
        { }

        public override string ClientId => "inspectors-mobile-client";

        protected override SecurityUser GetUser(string userName, UserSearchType userSearchType)
        {
            return securityService.GetUser(userName, userSearchType);
        }
    }
}
