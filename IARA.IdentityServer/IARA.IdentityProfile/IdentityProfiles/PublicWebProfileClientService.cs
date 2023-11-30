using IARA.Security.Enums;
using IARA.Security.Interfaces;
using IARA.Security.Models.SecurityModels;
using IARA.Security.Services;
using Microsoft.AspNetCore.Http;

namespace IARA.IdentityProfile.IdentityProfiles
{
    public class PublicWebProfileClientService : BaseProfileClientService
    {
        public PublicWebProfileClientService(PasswordHasher passwordHasher,
                                              ISecurityService securityService,
                                              SecurityEmailSender emailSenderQueue,
                                              IHttpContextAccessor httpContext)
            : base(passwordHasher, securityService, emailSenderQueue, httpContext)
        { }

        public override string ClientId => "public-web-client";

        protected override SecurityUser GetUser(string userName, UserSearchType userSearchType)
        {
            return securityService.GetUser(userName, userSearchType);
        }
    }
}
