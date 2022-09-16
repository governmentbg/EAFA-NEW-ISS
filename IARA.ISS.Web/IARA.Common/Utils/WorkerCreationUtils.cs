using System;
using System.Threading;
using System.Threading.Tasks;
using IARA.Common.Constants;
using Npgsql;
using TL.BatchWorkers;
using TL.BatchWorkers.Interfaces;
using TL.BatchWorkers.Models.Parameters.AsyncWorker;
using TLTTS.Common.ConfigModels;

namespace IARA.Common.Utils
{
    public static class WorkerCreationUtils
    {


        public static IAsyncWorkerTaskQueue<TRequest, TResult> CreateWorkerQueue<TRequest, TResult>(Func<TRequest, CancellationToken,
                                                                                                    Task<TResult>> taskAction,
                                                                                                    ConnectionStrings connection,
                                                                                                    string queueName,
                                                                                                    bool isUnique = false)
           where TRequest : class
        {
            //var settings = new DatabaseAsyncWorkerSettings<TRequest, TResult>(queueName, taskAction, DefaultConstants.DB_TABLE_NAME, connection.Connection);
            //var workerQueue = AsyncWorkerQueueBuilder.CreateDatabaseWorker<TRequest, NpgsqlConnection, TResult>(settings);
            var workerQueue = AsyncWorkerQueueBuilder.CreateInMemoryWorker(new LocalAsyncWorkerSettings<TRequest, TResult>(taskAction)
            {
                QueueName = queueName,
                IsUnique = isUnique
            });

            return workerQueue;
        }
    }
}
