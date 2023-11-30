using System;
using System.Collections.Generic;
using System.Text;

namespace TechnoLogica.Authentication.EAuth
{
    public static class EAuthDefaults
    {        
        //https://localhost:44354/Account/AuthenticateCertificate

        public const string AuthenticationScheme = "EAuthHandler";
        public static readonly string DisplayName = "Електронна автентикация";
        public static readonly string AuthorizationEndpoint = "https://eauthn.egov.bg:9445/eAuthenticator/eAuthenticator.seam";
        public static readonly string RequestServiceOID = "2.16.100.1.1.34.1.2.1.1.1";
        public static readonly string SystemProviderOID = "2.16.100.1.1.31.1.2";
        public static readonly string RedirectErrorURL = "External/Error";
    }
}
