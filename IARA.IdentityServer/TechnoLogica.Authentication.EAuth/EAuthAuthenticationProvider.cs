using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechnoLogica.Authentication.Common;
using TechnoLogica.Common;

namespace TechnoLogica.Authentication.EAuth
{
    public class EAuthAuthenticationProvider : IAuthenticationProvider
    {
        public void AddAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication().AddEAuth(
                opt =>
                {
                    AssignOptions(configuration, opt);
                });
            var part = new AssemblyPart(this.GetType().Assembly);

            services.AddControllersWithViews()
                .ConfigureApplicationPartManager(apm => apm.ApplicationParts.Add(part));
        }

        private static void AssignOptions(IConfiguration configuration, EAuthOptions opt)
        {
            var eAuthSettings = configuration.GetSettings<EAuthSettings>();
            opt.InformationSystemName = eAuthSettings.InformationSystemName;
            opt.ClientSystemCertificate = eAuthSettings.ClientSystemCertificate.GetX509Certificate2();
            opt.EAuthSystemCertificate = eAuthSettings.EAuthSystemCertificate.GetX509Certificate2();
        }

        public void BuildLogin(IConfiguration configuration, Controller controller, string returnURL, List<ExternalProvider> providers, ref bool externalOnly)
        {
            var eAuthProvider = providers.FirstOrDefault(p => p.AuthenticationScheme == EAuthDefaults.AuthenticationScheme);
            if (eAuthProvider != null)
            {
                string samlRequest = CreateSAMLRequest(configuration, controller.HttpContext);
                controller.ViewBag.EAuthHandlerSAMLRequest = samlRequest;
                controller.ViewBag.EAuthHandlerRelayState = StringUtils.Base64Encode(returnURL ?? "/");
                eAuthProvider.PartialView = "../Login/_EAuth.cshtml";
            }
        }

        public static string CreateSAMLRequest(IConfiguration configuration, HttpContext context)
        {
            var options = new EAuthOptions();
            AssignOptions(configuration, options);
            var request = Utils.GenerateNewSamlRequest(options, context);
            var signedXml = Utils.SignRequestWithCertificate(options, request);
            var isValid = Utils.IsValidSignatureXml(signedXml);
            if (isValid)
            {
                string base64EncodedSAMLRequest = StringUtils.Base64Encode(signedXml);
                return base64EncodedSAMLRequest;
            }
            else
            {
                throw new ApplicationException("Error validating the signed SAMLRequest");
            }
        }

        public async Task<IActionResult> Challenge(IConfiguration configuration, Controller controller, string provider, string returnUrl, string clientName)
        {
            return null;
        }

        public async Task<AuthenticateResult> ExternalCallback(AuthenticateResult result, HttpContext httpContext, Dictionary<string, string> additionalClaims)
        {
            if (result?.Succeeded != true)
            {
                result = await httpContext.AuthenticateAsync(EAuthDefaults.AuthenticationScheme);
            }
            return result;
        }

        public async Task SignOutAsync(HttpContext httpContext)
        {
            // Intentionally left blank. No specific SingOut logic needed.
        }
    }
}
