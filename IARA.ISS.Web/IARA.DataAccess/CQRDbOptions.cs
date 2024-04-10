using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using TL.AspNet.Security.Abstractions.Models;
using TL.Common.Settings;

namespace IARA.DataAccess
{
    public static class CQRSConnectionUtils
    {
        public static string GetConnectionString(IServiceProvider serviceProvider, ConnectionStrings connectionStrings = null)
        {
            var requestContextContainer = serviceProvider.GetService<RequestContextContainer>();

            connectionStrings ??= serviceProvider.GetService<ConnectionStrings>();

            string endpoint = requestContextContainer?.RequestContext?.Endpoint.ToLower();

            if (!string.IsNullOrEmpty(endpoint) && readMethodNames.Where(x => endpoint.StartsWith(x)).Any())
            {
                return connectionStrings.Failover;
            }
            else
            {
                return connectionStrings.Connection;
            }
        }


        public static HashSet<string> readMethodNames = new HashSet<string>
        {
            "get",
            "read",
            "download"
        };

        public static HashSet<string> readWriteMethodNames = new HashSet<string>
        {
            "add",
            "delete",
            "update",
            "edit",
            "restore",
            "undo",
            "manage"
        };
    }
}
