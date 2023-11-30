using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TechnoLogica.Authentication.Common
{
    public interface IAuthenticationProvider
    {
        public static string ADDITIONAL_CLAIM_NAME = "name";
        public static string ADDITIONAL_CLAIM_EMAIL = "email";
        void AddAuthentication(IServiceCollection services, IConfiguration configuration);
        void BuildLogin(IConfiguration configuration, Controller controller, string returnURL, List<ExternalProvider> providers, ref bool externalOnly);
        Task<IActionResult> Challenge(IConfiguration configuration, Controller controller, string provider, string returnUrl, string clientName);
        Task<AuthenticateResult> ExternalCallback(AuthenticateResult result, HttpContext httpContext, Dictionary<string, string> additionalClaims);
        Task SignOutAsync(HttpContext httpContext);
    }
}
