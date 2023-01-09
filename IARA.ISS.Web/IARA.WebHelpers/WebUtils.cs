using IARA.Common.Constants;
using Microsoft.AspNetCore.Http;

namespace IARA.WebHelpers
{
    public static class WebUtils
    {
        public static string GetRequestIPAddress(HttpContext context)
        {
            var headers = context.Request.Headers;
            string ip;

            if (headers.ContainsKey(DefaultConstants.X_FORWARDED_FOR))
            {
                ip = headers[DefaultConstants.X_FORWARDED_FOR].ToString();
            }
            else if (headers.ContainsKey(DefaultConstants.X_REAL_IP))
            {
                ip = headers[DefaultConstants.X_REAL_IP].ToString();
            }
            else
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }

            return ip;
        }
    }
}
