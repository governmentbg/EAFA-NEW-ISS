using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TechnoLogica.Authentication.EAuth
{
    public class EAuthOptions : RemoteAuthenticationOptions
    {
        /// <summary>
        /// Gets or sets the URI where the client will be redirected to authenticate.
        /// </summary>
        public string AuthorizationEndpoint { get; set; }

        public string InformationSystemName { get; set; }

        public string SystemProviderOID { get; set; }

        public string RequestServiceOID { get; set; }

        public X509Certificate2 ClientSystemCertificate { get; set; }

        public X509Certificate2 EAuthSystemCertificate { get; set; }

        public string RedirectErrorURL { get; set; }

        public EAuthOptions()
        {
            CallbackPath = "/EAuthCallback";
            AuthorizationEndpoint = EAuthDefaults.AuthorizationEndpoint;
            RequestServiceOID = EAuthDefaults.RequestServiceOID;
            SystemProviderOID = EAuthDefaults.SystemProviderOID;
            RedirectErrorURL = EAuthDefaults.RedirectErrorURL;
            Events = new RemoteAuthenticationEvents()
            { 
                OnRemoteFailure =
                    async (failureContext) => {
                        var items = failureContext?.Properties?.Items;
                        items = items ?? new Dictionary<string, string>();
                        var redirectWithQuery = QueryHelpers.AddQueryString(RedirectErrorURL, items);
                        failureContext.Response.Redirect(redirectWithQuery);
                        failureContext.HandleResponse();
                    }
            };
        }
    }
}
