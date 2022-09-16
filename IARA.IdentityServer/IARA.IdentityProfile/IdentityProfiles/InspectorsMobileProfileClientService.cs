using IARA.Security;
using IARA.Security.SecurityModels;
using IARA.Security.Services;
using Microsoft.AspNetCore.Http;

namespace IARA.IdentityProfile
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

        protected override SecurityUser GetUser(string userName, bool searchByPersonId = false)
        {
            return securityService.GetInternalUser(userName, searchByPersonId);
        }
    }
}
