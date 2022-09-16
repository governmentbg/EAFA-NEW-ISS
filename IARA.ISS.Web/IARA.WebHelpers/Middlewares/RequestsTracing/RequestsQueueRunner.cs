using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IARA.WebMiddlewares.RequestsTracing
{
    public abstract class RequestsQueueRunner<T>
    {
        private Task requestTask;
        protected readonly IConcurrentRequestQueue<T> requests;
        private readonly object requestsQueuePadlock;
        private readonly CancellationTokenSource cancellationTokenSource;
        private volatile bool isQueueEnabled;
        private object newItemEventPadlock;
        public uint RequestsQueueMaxLength { get; set; }
        public int QueueProcessingTime { get; set; }

        private object newItemEventHandlerPadlock;
        private object removeExcessPadlock;

        private event EventHandler newItemQueued = delegate { };

        public event EventHandler NewItemQueued
        {
            add
            {
                lock (newItemEventHandlerPadlock)
                {
                    newItemQueued += value;
                }
            }
            remove
            {
                lock (newItemEventHandlerPadlock)
                {
                    newItemQueued -= value;
                }
            }
        }

        protected abstract void ProcessQueueElement(T element);

        /// <summary>
        /// Request queue runner constructor
        /// </summary>
        /// <param name="maxQueueLength">If queue exceeds max length older records are dropped out</param>
        /// <param name="queueProcessingTime">The time in milliseconds per which the queue processor kicks in</param>
        public RequestsQueueRunner(uint maxQueueLength, int queueProcessingTime, IConcurrentRequestQueue<T> requests)
        {
            this.requestsQueuePadlock = new object();
            this.newItemEventHandlerPadlock = new object();
            this.removeExcessPadlock = new object();
            this.isQueueEnabled = false;
            this.cancellationTokenSource = new CancellationTokenSource();
            this.RequestsQueueMaxLength = maxQueueLength;
            this.QueueProcessingTime = queueProcessingTime;
            this.requests = requests;
            this.newItemEventPadlock = new object();
        }

        public Task Enqueue(T request)
        {
            if (isQueueEnabled)
            {
                return Task.Run(() =>
                 {
                     requests.Enqueue(request);
                     this.OnNewItemAdded();
                     if (requests.Count > RequestsQueueMaxLength)
                     {
                         lock (removeExcessPadlock)
                         {
                             while (requests.Count > RequestsQueueMaxLength)
                             {
                                 requests.TryDequeue(out T _);
                             }
                         }
                     }
                 });
            }

            return Task.CompletedTask;
        }

        public Task StartRequestsQueue()
        {
            if (requestTask == null)
            {
                lock (requestsQueuePadlock)
                {
                    if (requestTask == null)
                    {
                        requestTask = Task.Factory.StartNew((state) =>
                        {
                            while (!cancellationTokenSource.IsCancellationRequested)
                            {
                                if (requests.Any())
                                {
                                    QueueProcessor(requests);
                                }

                                Thread.Sleep(QueueProcessingTime);
                            }
                        }, TaskCreationOptions.LongRunning, cancellationTokenSource.Token);
                        isQueueEnabled = true;
                    }
                }
            }

            return requestTask;
        }

        public void StopRequestsQueue()
        {
            cancellationTokenSource.Cancel();
            requests.Clear();
            isQueueEnabled = false;
        }

        protected virtual void QueueProcessor(IConcurrentRequestQueue<T> queue)
        {
            while (queue.Any())
            {
                if (queue.TryDequeue(out T request))
                {
                    ProcessQueueElement(request);
                }
            }
        }

        private void OnNewItemAdded()
        {
            if (Monitor.TryEnter(this.newItemEventPadlock))
            {
                try
                {
                    foreach (var handler in this.newItemQueued.GetInvocationList())
                    {
                        handler.DynamicInvoke(this, EventArgs.Empty);
                    }
                }
                catch (Exception ex)
                {
                    //TODO Logging
                }
                finally
                {
                    Monitor.Exit(this.newItemEventPadlock);
                }
            }
        }
    }
}
