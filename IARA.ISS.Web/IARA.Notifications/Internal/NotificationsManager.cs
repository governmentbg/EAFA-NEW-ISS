using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IARA.Notifications.Enums;
using IARA.Notifications.Models;
using TL.BatchWorkers;
using TL.BatchWorkers.Interfaces;
using TL.BatchWorkers.Models.Parameters.AsyncWorker;

namespace IARA.Notifications
{
    internal class NotificationsManager : INotificationsManager
    {
        private readonly IAsyncWorkerTaskQueue<ClientNotification, bool> clientNotificationsQueue;
        private readonly IReadOnlyDictionary<NotificationTypes, LinkedList<string>> clientSubscriptions;
        private readonly IReadOnlyDictionary<NotificationTypes, IAsyncWorkerTaskQueue<BaseNotification, bool>> broadcastNotifications;
        private readonly IReadOnlyDictionary<NotificationTypes, object> padlocks;
        private readonly INotificationClients clients;

        public NotificationsManager(INotificationClients clients)
        {
            this.clients = clients;

            var clientSubscriptions = new Dictionary<NotificationTypes, LinkedList<string>>();
            var broadcastNotifications = new Dictionary<NotificationTypes, IAsyncWorkerTaskQueue<BaseNotification, bool>>();
            var padlocks = new Dictionary<NotificationTypes, object>();

            foreach (var enumValue in Enum.GetNames(typeof(NotificationTypes)))
            {
                NotificationTypes notificationType = Enum.Parse<NotificationTypes>(enumValue);

                var settings = new LocalAsyncWorkerSettings<BaseNotification, bool>(GetHandler(notificationType))
                {
                    QueueName = notificationType.ToString(),
                    IsUnique = false
                };

                broadcastNotifications.Add(notificationType, AsyncWorkerQueueBuilder.CreateInMemoryWorker(settings));
                clientSubscriptions.Add(notificationType, new LinkedList<string>());
                padlocks.Add(notificationType, new object());
            }

            this.clientSubscriptions = clientSubscriptions;
            this.broadcastNotifications = broadcastNotifications;
            this.padlocks = padlocks;


            var clientSettings = new LocalAsyncWorkerSettings<ClientNotification, bool>(HandleClientNotifications)
            {
                QueueName = nameof(this.clientNotificationsQueue),
                IsUnique = false
            };

            this.clientNotificationsQueue = AsyncWorkerQueueBuilder.CreateInMemoryWorker(clientSettings);
        }

        public Task<bool> AddClientNotification<T>(string clientId, Notification<T> notification)
        {
            if (clientSubscriptions[NotificationTypes.User].Contains(clientId))
            {
                return this.clientNotificationsQueue.Enqueue(new ClientNotification(clientId, notification));
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> AddBroadcastNotification<T>(Notification<T> notification)
        {
            if (clientSubscriptions[notification.Type].Any())
            {
                return broadcastNotifications[notification.Type].Enqueue(notification);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public void RemoveAllClientSubscriptions(Client client)
        {
            foreach (var type in Enum.GetNames(typeof(NotificationTypes)))
            {
                NotificationTypes notificationType = Enum.Parse<NotificationTypes>(type);

                if (client.TryRemove(notificationType, out LinkedListNode<string> subscription))
                {
                    lock (this.padlocks[notificationType])
                    {
                        clientSubscriptions[notificationType].Remove(subscription);
                    }
                }
            }
        }

        public bool SubscribeForEvent(string clientId, NotificationTypes type)
        {
            lock (this.padlocks[type])
            {
                LinkedListNode<string> subscription = clientSubscriptions[type].AddLast(clientId);
                return clients[clientId].TryAdd(type, subscription);
            }
        }

        public bool UnsubscribeFromEvent(string clientId, NotificationTypes type)
        {
            lock (this.padlocks[type])
            {
                if (clients[clientId].TryRemove(type, out var subscription))
                {
                    clientSubscriptions[type].Remove(subscription);
                    return true;
                }

                return false;
            }
        }

        private Func<BaseNotification, CancellationToken, Task<bool>> GetHandler(NotificationTypes type)
        {
            switch (type)
            {
                case NotificationTypes.User:
                    return HandleUserNotifications;
                case NotificationTypes.ReportDownload:
                    return HandleReportDownload;
                case NotificationTypes.ReportExecution:
                    return HandleReportExecution;
                case NotificationTypes.NomenclatureUpdate:
                    return HandleNomenclatureUpdate;
                default:
                    throw new ArgumentException();
            }
        }

        private string GetMethodHandler(NotificationTypes type)
        {
            return $"Receive{type}";
        }

        private Task<bool> HandleClientNotifications(ClientNotification notification, CancellationToken token)
        {
            Client client = clients[notification.ClientId];

            if (client != null)
            {
                return client.SendAsync(GetMethodHandler(notification.Notification.Type), notification.Notification, token).ContinueWith(t =>
                 {
                     if (t.IsCompletedSuccessfully)
                     {
                         return true;
                     }

                     return false;
                 });
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        private Task<bool> HandleNomenclatureUpdate(BaseNotification notification, CancellationToken token)
        {
            return SendToAllClientsOfType(notification, token);
        }

        private Task<bool> HandleReportDownload(BaseNotification notification, CancellationToken token)
        {
            return SendToAllClientsOfType(notification, token);
        }

        private Task<bool> HandleReportExecution(BaseNotification notification, CancellationToken token)
        {
            return SendToAllClientsOfType(notification, token);
        }

        private Task<bool> HandleUserNotifications(BaseNotification notification, CancellationToken token)
        {
            return SendToAllClientsOfType(notification, token);
        }

        private Task<bool> SendToAllClientsOfType(BaseNotification notification, CancellationToken token)
        {
            NotificationTypes type = notification.Type;

            List<Task<bool>> clientTasks = new List<Task<bool>>();

            foreach (var clientId in clientSubscriptions[type])
            {
                var task = clients[clientId].SendAsync(GetMethodHandler(type), notification, token).ContinueWith(t =>
                {
                    if (t.IsCompletedSuccessfully)
                    {
                        return true;
                    }

                    return false;
                });

                clientTasks.Add(task);
            }

            return Task.WhenAll(clientTasks).ContinueWith(t =>
            {
                if (t.IsCompletedSuccessfully)
                {
                    return t.Result.All(x => x);
                }

                return false;
            });
        }

    }
}
