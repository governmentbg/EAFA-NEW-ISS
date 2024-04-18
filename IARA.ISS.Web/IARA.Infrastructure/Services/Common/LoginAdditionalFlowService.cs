using IARA.Security.Interfaces;
using TL.AspNet.Security.Abstractions.Models;
using TL.AspNet.Security.Abstractions.Services;
using TL.AspNet.Security.Abstractions.User;

namespace IARA.Infrastructure.Services.Common
{
    public class LoginAdditionalFlowService : ILoginAdditionalFlowService<string>
    {
        private ISecurityService securityService;
        public LoginAdditionalFlowService(ISecurityService securityService)
        {
            this.securityService = securityService;
        }

        public void AfterFailedLogin(UserContext<string> userContext, string userIdentifier)
        {
            //NO LOGIC NEEDED
        }

        public void AfterSuccessfulLogin(UserContext<string> userContext, string userIdentifier)
        {
            securityService.UpdateUserRoles((userContext.User as IUser<string>)?.Username ?? userContext.Id, userContext.LoginType.HasValue ? userContext.LoginType.Value : userContext.LoginType.Value);
        }
    }
}
