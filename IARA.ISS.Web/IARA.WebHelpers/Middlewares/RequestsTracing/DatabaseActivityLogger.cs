using System;
using System.Linq;
using IARA.Interfaces;
using IARA.Logging.Abstractions.Interfaces;
using IARA.WebMiddlewares.RequestsTracing.Models;
using IARA.WebMiddlewares.RequestsTracing.RequestCollections;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.WebMiddlewares.RequestsTracing
{
    public sealed class DatabaseActivityLogger : RequestsQueueRunner<RequestUser>, IDisposable
    {
        public const int DEFAULT_QUEUE_PROCESSING_TIME_MS = 60000;
        public const int DEFAULT_QUEUE_MAX_LENGTH = 1000;

        public DatabaseActivityLogger(IServiceProvider serviceProvider)
            : base(DEFAULT_QUEUE_MAX_LENGTH, DEFAULT_QUEUE_PROCESSING_TIME_MS, new ConcurrentUniqueQueue<RequestUser>())
        {
            this.serviceProvider = serviceProvider;
        }

        private IServiceProvider serviceProvider;

        private static volatile bool loggingEnabled;

        public static bool IsLoggingEnabled
        {
            get
            {
                return loggingEnabled;
            }
        }

        public bool LoggingEnabled
        {
            get
            {
                return loggingEnabled;
            }
            set
            {
                if (loggingEnabled != value)
                {
                    loggingEnabled = value;

                    if (loggingEnabled)
                    {
                        StartRequestsQueue();
                    }
                    else
                    {
                        StopRequestsQueue();
                    }
                }
            }
        }

        protected override void ProcessQueueElement(RequestUser element)
        {
            throw new NotImplementedException();
        }

        protected override void QueueProcessor(IConcurrentRequestQueue<RequestUser> queue)
        {
            using (IUserService userService = serviceProvider.GetRequiredService<IUserService>())
            {
                try
                {
                    var usersDict = queue.ToDictionary(x => x.Username, x => x.TimeOfRequest);
                    userService.UpdateUsersActivity(usersDict);
                }
                catch (Exception ex)
                {
                    var exceptionService = serviceProvider.GetRequiredService<IExtendedLogger>();
                    exceptionService.LogException(ex);
                }
            }
        }

        public void Dispose()
        {
            this.serviceProvider = null;
        }
    }
}
