using System;
using System.Net;
using System.Security.Claims;
using System.Threading;
using IARA.Common.Constants;
using IARA.Logging.Abstractions.Models;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace IARA.WebHelpers.Filters
{
    public static class FilterExtensions
    {
        public static void SetCurrentPrincipal(this ActionExecutingContext context, bool shouldMarkAsAuthenticated, string defaultUserName = null)
        {
            ClaimsIdentity claimsIdentity = null;
            bool? isAuthenticated = context.HttpContext?.User?.Identity?.IsAuthenticated;

            if (!isAuthenticated.HasValue || !isAuthenticated.Value)
            {
                if (defaultUserName != null)
                {
                    claimsIdentity = new ExtendedClaimsIdentity(shouldMarkAsAuthenticated);
                    claimsIdentity.AddClaim(new Claim(JwtClaimTypes.Name, defaultUserName));
                    claimsIdentity.AddClaim(new Claim(JwtClaimTypes.Id, DefaultConstants.SYSTEM_USER_ID.ToString()));
                }
            }
            else
            {
                claimsIdentity = context.HttpContext?.User?.Identity as ClaimsIdentity;
            }

            if (claimsIdentity != null)
            {
                claimsIdentity.AddIdentificationClaims(context.HttpContext.GetRequestContext());
                context.HttpContext.User = new ClaimsPrincipal(claimsIdentity);
                Thread.CurrentPrincipal = context.HttpContext.User;
            }
        }


        public static RequestData GetRequestContext(this HttpContext httpContext)
        {
            IPAddress remoteIp = httpContext.Connection.RemoteIpAddress;
            IHeaderDictionary headers = httpContext.Request.Headers;
            string ip = headers.TryGetValue(DefaultConstants.FORWARDED_FOR, out StringValues forwarded) ? forwarded.ToString() : remoteIp.ToString();
            string endpoint = httpContext.Request.Path.ToUriComponent();
            string browserInfo = headers.TryGetValue(DefaultConstants.USER_AGENT, out StringValues userAgent) ? userAgent.ToString() : string.Empty;

            return new RequestData
            {
                BrowserInfo = browserInfo,
                Endpoint = endpoint,
                IPAddress = ip,
                TimeOfRequest = DateTime.Now
            };
        }

        public static void AddIdentificationClaims(this ClaimsIdentity identity, RequestData data)
        {
            identity.AddClaim(new Claim(DefaultConstants.IP, data.IPAddress));
            identity.AddClaim(new Claim(DefaultConstants.ENDPOINT, data.Endpoint));
            identity.AddClaim(new Claim(DefaultConstants.BROWSER_INFO, data.BrowserInfo));
        }
    }

    public class ExtendedClaimsIdentity : ClaimsIdentity
    {
        public ExtendedClaimsIdentity(bool isAuthenticated)
        {
            this.authenticated = isAuthenticated;
        }

        private bool authenticated;

        public override bool IsAuthenticated => authenticated;
    }


}
