using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using IARA.Logging.Abstractions.Models;
using IARA.WebMiddlewares.RequestsTracing.Models;
using IARA.WebMiddlewares.RequestsTracing.RequestCollections;

namespace IARA.WebMiddlewares.RequestsTracing
{
    public sealed class InMemoryStatisticsLogger : RequestsQueueRunner<RequestData>, IEnumerable<KeyValuePair<string, Statistics>>
    {
        public const int DEFAULT_QUEUE_PROCESSING_TIME_MS = 500;
        public const int DEFAULT_QUEUE_MAX_LENGTH = 2000;

        public static uint MinutesToKeepStats { get; set; } = 2;

        private readonly ConcurrentDictionary<string, Statistics> categoryDictionary;
        private readonly object timerPadlock;
        private readonly System.Timers.Timer timer;

        private static volatile bool tracingEnabled;

        public static bool IsTracingEnabled
        {
            get
            {
                return tracingEnabled;
            }
        }

        public bool TracingEnabled
        {
            get
            {
                return tracingEnabled;
            }
            set
            {
                if (tracingEnabled != value)
                {
                    tracingEnabled = value;
                    if (tracingEnabled)
                    {
                        StartRequestsQueue();
                        timer.Start();
                    }
                    else
                    {
                        StopRequestsQueue();
                    }
                }
            }
        }

        public InMemoryStatisticsLogger()
            : base(DEFAULT_QUEUE_MAX_LENGTH, DEFAULT_QUEUE_PROCESSING_TIME_MS, new ConcurrentRequestQueue<RequestData>())
        {
            categoryDictionary = new ConcurrentDictionary<string, Statistics>();
            timer = new System.Timers.Timer();
            timer.AutoReset = false;
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = TimeSpan.FromMinutes(MinutesToKeepStats).TotalMilliseconds;
            timerPadlock = new object();
        }

        protected override void ProcessQueueElement(RequestData element)
        {
            AddOrUpdateStatistics(RequestCategoryTypes.IP, element.IPAddress, element.TimeOfRequest);
            AddOrUpdateStatistics(RequestCategoryTypes.ENDPOINT, element.Endpoint, element.TimeOfRequest);
            AddOrUpdateStatistics(RequestCategoryTypes.USER, element.Username, element.TimeOfRequest);
        }

        private void AddOrUpdateStatistics(string categoryName, string statValue, DateTime exactTimeOfImpression)
        {
            categoryDictionary.AddOrUpdate(categoryName, new Statistics(statValue, exactTimeOfImpression), (category, value) =>
            {
                value.AddOrUpdate(statValue, exactTimeOfImpression);
                return value;
            });
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Monitor.TryEnter(timerPadlock))
            {
                timer.Stop();
                try
                {
                    DateTime time = DateTime.Now.AddMinutes(-MinutesToKeepStats);
                    foreach (var category in categoryDictionary)
                    {
                        category.Value.RemoveAll(time);
                    }
                }
                finally
                {
                    Monitor.Exit(timerPadlock);
                    timer.Start();
                }
            }
        }

        public IEnumerator<KeyValuePair<string, Statistics>> GetEnumerator()
        {
            return categoryDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
