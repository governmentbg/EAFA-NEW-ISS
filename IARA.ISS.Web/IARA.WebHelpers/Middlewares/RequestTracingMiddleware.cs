using System;
using System.Net;
using System.Threading.Tasks;
using IARA.Common.ConfigModels;
using IARA.Logging.Abstractions.Models;
using IARA.WebMiddlewares.RequestsTracing;
using IARA.WebMiddlewares.RequestsTracing.Models;
using Microsoft.AspNetCore.Http;

namespace IARA.WebMiddlewares
{
    public class RequestTracingMiddleware
    {
        private readonly RequestDelegate _next;
        private const string ANONYMOUS = nameof(ANONYMOUS);

        private DatabaseActivityLogger databaseActivityLogger;
        private InMemoryStatisticsLogger inMemoryActivityLogger;

        // Must have constructor with this signature, otherwise exception will be thrown at run time
        public RequestTracingMiddleware(RequestDelegate next,
            LoggingSettings settings,
            DatabaseActivityLogger databaseActivityLogger,
            InMemoryStatisticsLogger inMemoryActivityLogger)
        {
            this._next = next;
            this.databaseActivityLogger = databaseActivityLogger;
            this.inMemoryActivityLogger = inMemoryActivityLogger;
            databaseActivityLogger.LoggingEnabled = settings.ActivityLoggingEnabled;
        }

        public Task Invoke(HttpContext httpContext)
        {
            if (InMemoryStatisticsLogger.IsTracingEnabled || DatabaseActivityLogger.IsLoggingEnabled)
            {
                DateTime now = DateTime.Now;
                IHeaderDictionary headers = httpContext.Request.Headers;
                IPAddress remoteIp = httpContext.Connection.RemoteIpAddress;
                string ip = headers.ContainsKey("X-Forwarded-For") ? headers["X-Forwarded-For"].ToString() : remoteIp.ToString();
                string username = httpContext.User.Identity?.Name ?? ANONYMOUS;
                string endpoint = httpContext.Request.Path.ToUriComponent();

                if (InMemoryStatisticsLogger.IsTracingEnabled)
                {
                    inMemoryActivityLogger.Enqueue(new RequestData
                    {
                        Endpoint = endpoint,
                        Username = username,
                        IPAddress = ip,
                        TimeOfRequest = now
                    });
                }

                if (DatabaseActivityLogger.IsLoggingEnabled)
                {
                    if (username != ANONYMOUS)
                    {
                        databaseActivityLogger.Enqueue(new RequestUser(username, now));
                    }
                }
            }

            return _next.Invoke(httpContext);
        }
    }
}
