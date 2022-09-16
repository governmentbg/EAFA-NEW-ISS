using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IARA.Common.ConfigModels;
using IARA.Logging.Abstractions.Interfaces;
using IARA.Logging.Abstractions.Models;
using Microsoft.Extensions.Logging;
using TL.BatchWorkers;
using TL.BatchWorkers.Interfaces;
using TL.BatchWorkers.Models.Parameters.AsyncWorker;

namespace IARA.Logging.TeamsLogging
{
    public class TeamsLogger : ITeamsLogger
    {
        private object padlock = new object();
        private IAsyncWorkerTaskQueue<LogRecord, bool> taskQueue;
        private string teamsWebhookUrl;
        public TeamsLogger(LoggingSettings loggingSettings)
        {
            LogLevel = loggingSettings.LogLevel.Teams;
            teamsWebhookUrl = loggingSettings.TeamsWebHookUrl;
            taskQueue = AsyncWorkerQueueBuilder.CreateInMemoryWorker(new LocalAsyncWorkerSettings<LogRecord, bool>(Log));
        }

        public LogLevel LogLevel { get; private set; }

        public Task<bool> Log(LogRecord record)
        {
            if (LoggingUtils.IsEnabled(record.Level, LogLevel))
            {
                lock (padlock)
                {
                    return taskQueue.Enqueue(record);
                }
            }

            return Task.FromResult(false);
        }

        private string GetJsonForTeamsCard(LogRecord record)
        {
            List<Fact> facts = new List<Fact>
            {
                new Fact { Name = "User:", Value = record.Username },
                new Fact { Name = "Source:", Value = record.ExceptionSource },
                new Fact { Name = "Message:", Value = record.Message },
                new Fact { Name = "Stack Trace:", Value = LoggingUtils.FilterStackTrace(record.StackTrace) ?? "NONE"}
            };

            TeamsErrorMessageModel message = new TeamsErrorMessageModel
            {
                Summary = "Error in " + record.ExceptionSource,
                Sections = new Section[]
                {
                   new Section
                   {
                      ActivityTitle = "Error " + record.Message,
                      ActivitySubtitle = "Source: " + record.ExceptionSource,
                      Facts = facts
                   }
                }
            };

            return JsonSerializer.Serialize(message);
        }

        private Task<bool> Log(LogRecord record, CancellationToken cancellationToken)
        {
            string json = GetJsonForTeamsCard(record);
            return SendToWebhook(json, cancellationToken);
        }

        private async Task<bool> SendToWebhook(string json, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(teamsWebhookUrl))
            {
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.PostAsync(teamsWebhookUrl, new StringContent(json), cancellationToken);
                        response.EnsureSuccessStatusCode();
                    }

                    return true;
                }
                catch (Exception x)
                {
                    Console.Error.WriteLine(x.Message);
                    return false;
                }
            }

            return false;
        }
    }
}
