using System;
using TechnoLogica.Authentication.Common;

namespace TechnoLogica.IdentityServer
{
    public class IdentityServer
    {
        public string[] CompositionModules { get; set; }
        public string CspFrameAncestors { get; set; }
        public string CspJSHash { get; set; }
        public bool EnablePasswordReset { get; set; } = true;
        public CORSSettings CORS { get; set; }
        public ClientPostLogoutRedirect[] ClientPostLogoutRedirects { get; set; }
        public string MappingPath { get; set; }
        public bool SSLOffloaded { get; set; }
        public bool UseForwardHeaders { get; set; }
        public CertificateSettings SigningCredential { get; set; }
        public bool AllowLocalLogin { get; set; } = true;
        public bool AllowRememberLogin { get; set; } = false;
        public TimeSpan RememberMeLoginDuration { get; set; } = TimeSpan.FromDays(30);
        public bool ShowLogoutPrompt { get; set; } = true;
        public bool AutomaticRedirectAfterSignOut { get; set; } = false;
        public TimeSpan? CookieLifetime { get; set; }
        public bool? CookieSlidingExpiration { get; set; }
    }

    public class CORSSettings
    {
        public bool Enabled { get; set; }
        public string[] Origins { get; set; }
        public string[] Methods { get; set; }
        public string[] Headers { get; set; }
    }

    public class ClientPostLogoutRedirect
    {
        public string ClientId { get; set; }
        public string PostLogoutRedirectUrl { get; set; }
    }
}
