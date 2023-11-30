using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Xml;
using TechnoLogica.Authentication.Common;
using TechnoLogica.Authentication.EAuth.XSD;

namespace TechnoLogica.Authentication.EAuth
{
    public class EAuthHandler : RemoteAuthenticationHandler<EAuthOptions>
    {
        private IHostingEnvironment _hostingEnvironment;
        public EAuthHandler(
            IHostingEnvironment hostingEnvironment,
            IOptionsMonitor<EAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
            ) : base(options, logger, encoder, clock)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            RedirectContext<EAuthOptions> redirectContext =
                new RedirectContext<EAuthOptions>(
                    Context,
                    Scheme,
                    Options,
                    properties,
                    Options.AuthorizationEndpoint);
            redirectContext.Response.StatusCode = 307;
            redirectContext.Response.Headers[HeaderNames.Location] = Options.AuthorizationEndpoint;
        }

        protected async override Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            var urlReferer = Request.Headers["Referer"].ToString();
            var hostUri = new Uri(Options.AuthorizationEndpoint);
            var expectedHostName = $"{hostUri.Scheme}://{hostUri.Host}".ToLower();

            if (string.IsNullOrEmpty(urlReferer) ||
                !urlReferer.ToLower().StartsWith(expectedHostName))
            {
                var authProperties = new AuthenticationProperties
                {
                    RedirectUri = "External/Callback",
                    Items =
                        {
                            { "scheme", this.Scheme.Name }
                        }
                };
                authProperties.Items.Add(
                    "error_messages", 
                    $"Неуспешна идентификация. Възникна проблем с разчитането на Вашия КЕП.{ Environment.NewLine}" +
                    $"Заявката не идва от очакваното място.{ Environment.NewLine}"+
                    $"Адрес: \"{urlReferer}\"");

                return HandleRequestResult.Fail("Unexpected hostname", authProperties);
            }

            var samlResponseInput = Request.Form["SAMLResponse"];
            var returnUrlBas64 = Request.Form["RelayState"];

            var samlXml = StringUtils.Base64Decode(samlResponseInput);
            var returnUrl = StringUtils.Base64Decode(returnUrlBas64);

            var props = new AuthenticationProperties
            {
                RedirectUri = "External/Callback",
                Items =
                    {
                        { "returnUrl", returnUrl },
                        { "scheme", this.Scheme.Name }
                    }
            };

            var isValidXml = XmlUtils.IsValidXml(samlXml);
            if (!isValidXml)
            {
                props.Items.Add(
                    "error_messages",
                    $"Неуспешна идентификация.Възникна проблем с разчитането на Вашия КЕП.{Environment.NewLine}" +
                    $"Резултатът от услугата не е валиден xml.");
                props.Items.Add(
                    "error_hidden_messages",
                    samlXml);

                return HandleRequestResult.Fail("InvalidSignature", props);
            }

            //TODO: Fix certificate reading...
            var eauthCert = Options.EAuthSystemCertificate;
            var isValid = Utils.IsValidSignatureXml(samlXml, eauthCert);
            if (!isValid)
            {
                props.Items.Add(
                    "error_messages",
                    $"Неуспешна идентификация. Възникна проблем с разчитането на Вашия КЕП.{Environment.NewLine}" +
                    $"Подписът е невалиден.");
                return HandleRequestResult.Fail("InvalidSignature", props);
            }

            var samlResponse = XmlUtils.DeserializeXml<ResponseType>(samlXml);
            var statusCode = samlResponse.Status.StatusCode;
            if (!Utils.IsValidStatus(statusCode))
            {
                props.Items.Add(
                    "error_messages",
                    $"Неуспешна идентификация. Възникна проблем с разчитането на Вашия КЕП.{Environment.NewLine}" +
                    $"Пробвайте да извадите устройството, да рестартирате браузъра, да поставите устройството и да опитате отново.{Environment.NewLine}" +
                    $"Ако проблемите продължават, свържете с администратора на системата.");
                props.Items.Add(
                    "error_hidden_messages",
                    $"STS съобщение: {samlResponse.Status.StatusMessage}");
                return HandleRequestResult.Fail("InvalidSignature", props);
            }

            var samlXMLDocument = new XmlDocument();
            samlXMLDocument.LoadXml(samlXml);

            var egn = Utils.ExtractEgn(samlXMLDocument);
            if (egn == null)
            {
                props.Items.Add(
                      "error_messages",
                      $"Неуспешна идентификация. Възникна проблем с разчитането на Вашия КЕП.{Environment.NewLine}" +
                      $"Не може да се извлече ЕГН от резултата.{Environment.NewLine}");
                return HandleRequestResult.Fail("InvalidSignature", props);
            }

            var name = Utils.ExtractName(samlXMLDocument);
            var email = Utils.ExtractEmail(samlXMLDocument);


            var identity = new ClaimsIdentity(ClaimsIssuer);
            identity.AddClaim(new Claim(JwtClaimTypes.Subject, egn));
            if (!string.IsNullOrEmpty(email))
            {
                identity.AddClaim(new Claim(EAuthClaims.EMail, email));
            }
            if (!string.IsNullOrEmpty(name))
            {
                identity.AddClaim(new Claim(EAuthClaims.PersonNamesLatin, name));
            }


            return HandleRequestResult.Success(
                new AuthenticationTicket(
                    new ClaimsPrincipal(identity),
                    props,
                    this.Scheme.Name)
            );
        }
    }
}
