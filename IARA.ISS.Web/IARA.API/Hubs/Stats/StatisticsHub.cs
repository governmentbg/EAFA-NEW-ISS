using System;
using System.Threading.Tasks;
using IARA.WebHelpers.Hubs.Stats;
using IARA.WebMiddlewares.RequestsTracing;
using Microsoft.AspNetCore.SignalR;

namespace IARA.WebAPI.Hubs.Stats
{
    public class StatisticsHub : Hub
    {
        private StastisticsWorker worker;
        private InMemoryStatisticsLogger inMemoryLogger;

        public StatisticsHub(StastisticsWorker worker, InMemoryStatisticsLogger inMemoryLogger)
        {
            this.worker = worker;
            this.inMemoryLogger = inMemoryLogger;
            // this.worker.Clients = this.Clients;
        }

        public override Task OnConnectedAsync()
        {
            this.worker.Clients.Add(this.Context.ConnectionId, this.Clients.Caller);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = this.Context?.ConnectionId;
            if (!string.IsNullOrEmpty(connectionId))
            {
                this.worker.Clients.Remove(this.Context.ConnectionId, out _);
                worker.RemoveFromRawUsageData(connectionId);
                worker.RemoveFromStatisticsListeners(connectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public async Task<string> GetRawUsageData(bool startStop)
        {
            if (startStop)
            {
                this.worker.AddToRawUsageDataListeners(this.Context.ConnectionId);
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, nameof(GetRawUsageData));
            }
            else
            {
                worker.RemoveFromRawUsageData(this.Context.ConnectionId);
            }

            return $"User added to {nameof(GetStatistics)}";
        }

        public string EnableTracing(bool enable)
        {
            this.inMemoryLogger.TracingEnabled = enable;
            string message = inMemoryLogger.TracingEnabled ? "Tracing enabled" : "Tracing disabled";
            return message;
        }

        public bool IsTracingEnabled()
        {
            return this.inMemoryLogger.TracingEnabled;
        }

        public async Task<string> GetStatistics(bool startStop)
        {
            if (startStop)
            {
                this.worker.AddToStatisticsListeners(this.Context.ConnectionId);
                await this.Groups.AddToGroupAsync(this.Context.ConnectionId, nameof(GetStatistics));
            }
            else
            {
                worker.RemoveFromStatisticsListeners(this.Context.ConnectionId);
            }

            return $"User added to {nameof(GetStatistics)}";
        }
    }
}
