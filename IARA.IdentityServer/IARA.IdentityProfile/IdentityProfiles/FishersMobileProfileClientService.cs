using IARA.Security;
using IARA.Security.SecurityModels;
using IARA.Security.Services;
using Microsoft.AspNetCore.Http;

namespace IARA.IdentityProfile
{
    public class FishersMobileProfileClientService : BaseProfileClientService
    {
        public FishersMobileProfileClientService(PasswordHasher passwordHasher,
                                                 ISecurityService securityService,
                                                 SecurityEmailSender emailSenderQueue,
                                                 IHttpContextAccessor httpContext)
          : base(passwordHasher, securityService, emailSenderQueue, httpContext)
        { }

        public override string ClientId => "fishermen-mobile-client";

        protected override SecurityUser GetUser(string userName, bool searchByPersonId = false)
        {
            return securityService.GetUser(userName, searchByPersonId);
        }
    }
}
