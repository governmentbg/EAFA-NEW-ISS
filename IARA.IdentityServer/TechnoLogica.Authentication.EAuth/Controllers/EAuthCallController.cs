using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TechnoLogica.Common;

namespace TechnoLogica.Authentication.EAuth.Controllers
{
    [AllowAnonymous]
    public class EAuthCallController : Controller
    {
        private EAuthSettings EAuthSettings { get; set; }
        private IConfiguration Configuration { get; set; }
        private IIdentityServerInteractionService Interaction { get; set; }
        private readonly IClientStore ClientStore;

        public EAuthCallController(IConfiguration configuration,
                                   IIdentityServerInteractionService interaction,
                                   IClientStore clientStore)
        {
            EAuthSettings = configuration.GetSettings<EAuthSettings>();
            Configuration = configuration;
            Interaction = interaction;
            ClientStore = clientStore;
        }

        [HttpPost]
        [ActionName("Post")]
        public async Task<IActionResult> Index(string returnUrl)
        {
            string samlRequest = EAuthAuthenticationProvider.CreateSAMLRequest(Configuration, Request.HttpContext);
            ViewBag.AuthenticationScheme = EAuthDefaults.AuthenticationScheme;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.EAuthHandlerSAMLRequest = samlRequest;
            ViewBag.EAuthHandlerRelayState = StringUtils.Base64Encode(returnUrl ?? "/");
            var context = await Interaction.GetAuthorizationContextAsync(returnUrl);
            if (context != null)
            {
                if (context?.Client.ClientId != null)
                {
                    var client = await ClientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                    ViewBag.ClientName = client?.ClientName;
                }
            }
            return View("/Views/Login/_EAuthCall.cshtml");
        }
    }
}
