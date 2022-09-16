using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using IARA.Common.Constants;
using IARA.WebMiddlewares.RequestsTracing;
using IARA.WebMiddlewares.RequestsTracing.Models;
using Microsoft.AspNetCore.SignalR;

namespace IARA.WebHelpers.Hubs.Stats
{
    public class StastisticsWorker
    {
        private object dataTimerPadlock;
        private volatile bool hasNewData;
        private InMemoryStatisticsLogger inMemoryStatLogger;
        private volatile bool isTimerStarted;
        private System.Timers.Timer newDataTimer;
        private List<string> rawUsageClients;
        private List<string> statisticsClients;

        public StastisticsWorker(InMemoryStatisticsLogger inMemoryStatLogger)
        {
            this.inMemoryStatLogger = inMemoryStatLogger;
            this.newDataTimer = new System.Timers.Timer();
            this.dataTimerPadlock = new object();
            this.rawUsageClients = new List<string>();
            this.statisticsClients = new List<string>();
            this.Clients = new Dictionary<string, IClientProxy>();
            this.newDataTimer.Interval = 200;
            this.newDataTimer.Elapsed += NewDataTimer_Elapsed;
            this.inMemoryStatLogger.NewItemQueued += InMemoryStatLogger_NewItemQueued;
        }

        public Dictionary<string, IClientProxy> Clients { get; set; }

        public void AddToRawUsageDataListeners(string connectionID)
        {
            this.rawUsageClients.Add(connectionID);
            this.StartTimer();
        }

        public void AddToStatisticsListeners(string connectionID)
        {
            this.statisticsClients.Add(connectionID);
            this.StartTimer();
        }

        public IEnumerable<Category> GetRawUsageData()
        {
            var result = (from category in inMemoryStatLogger
                          select new Category
                          {
                              Name = category.Key,
                              Summaries = (from x in category.Value
                                           select new Summary
                                           {
                                               Value = x.Key,
                                               FirstImpression = x.Value.FirstImpression.ToString(DateTimeFormats.TIME_FORMAT),
                                               LastImpression = x.Value.LastImpresion.ToString(DateTimeFormats.TIME_FORMAT),
                                               Count = x.Value.Count
                                           }).ToList()
                          }).ToList();

            return result;
        }

        public IEnumerable<RequestStats> GetStatistics()
        {
            var temp = (from category in inMemoryStatLogger
                        select new
                        {
                            Category = category.Key,
                            Statistics = category.Value.SelectMany(x => x.Value.Select(y => new
                            {
                                Value = x.Key,
                                ExactTime = y.ToString(DateTimeFormats.TIME_FORMAT),
                                EnteredDateTime = y
                            })).ToList()
                        }).ToList();

            var result = (from ip in temp.Where(x => x.Category == RequestCategoryTypes.IP).SelectMany(x => x.Statistics)
                          join endpoint in temp.Where(x => x.Category == RequestCategoryTypes.ENDPOINT).SelectMany(x => x.Statistics)
                          on ip.ExactTime equals endpoint.ExactTime
                          join user in temp.Where(x => x.Category == RequestCategoryTypes.USER).SelectMany(x => x.Statistics)
                          on ip.ExactTime equals user.ExactTime
                          orderby user.EnteredDateTime descending
                          select new RequestStats
                          {
                              IpAddress = ip.Value,
                              Endpoint = endpoint.Value,
                              User = user.Value,
                              ExactTime = ip.ExactTime
                          });

            return result;
        }

        public void RemoveFromRawUsageData(string connectionID)
        {
            this.rawUsageClients.Remove(connectionID);
            StopTimer();
        }

        public void RemoveFromStatisticsListeners(string connectionID)
        {
            this.statisticsClients.Remove(connectionID);
            StopTimer();
        }

        private void InMemoryStatLogger_NewItemQueued(object sender, EventArgs e)
        {
            hasNewData = true;
        }

        private void NewDataTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Monitor.TryEnter(dataTimerPadlock))
            {
                this.newDataTimer.Stop();
                try
                {
                    if (hasNewData)
                    {
                        hasNewData = false;

                        if (this.rawUsageClients.Any())
                        {
                            var rawUsage = this.GetRawUsageData();
                            SendToGroup("GetRawUsageData", rawUsage);
                        }

                        if (this.statisticsClients.Any())
                        {
                            var statistics = this.GetStatistics();
                            SendToGroup("GetStatistics", statistics);
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(dataTimerPadlock);
                    this.newDataTimer.Start();
                }
            }
        }

        private void SendToGroup(string groupName, object data)
        {
            if (this.Clients != null)
            {
                switch (groupName)
                {
                    case nameof(GetStatistics):
                        {
                            foreach (var client in this.Clients.Where(x => statisticsClients.Contains(x.Key)))
                            {
                                client.Value.SendAsync(groupName, data);
                            }
                        }
                        break;

                    case nameof(GetRawUsageData):
                        {
                            foreach (var client in this.Clients.Where(x => rawUsageClients.Contains(x.Key)))
                            {
                                client.Value.SendAsync(groupName, data);
                            }
                        }
                        break;

                    default:
                        throw new NotImplementedException("Unknown group name");
                }
            }
        }

        private bool StartTimer()
        {
            if (!isTimerStarted)
            {
                this.newDataTimer.Start();
                isTimerStarted = true;
            }

            return isTimerStarted;
        }

        private bool StopTimer()
        {
            if (!this.statisticsClients.Any() && !this.rawUsageClients.Any())
            {
                this.newDataTimer.Stop();
                isTimerStarted = false;
            }

            return isTimerStarted;
        }
    }
}
