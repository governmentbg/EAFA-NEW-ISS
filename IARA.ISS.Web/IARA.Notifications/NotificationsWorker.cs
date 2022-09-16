using System.Threading.Tasks;
using IARA.Notifications.Enums;
using IARA.Notifications.Interfaces;
using IARA.Notifications.Models;
using Microsoft.AspNetCore.SignalR;

namespace IARA.Notifications
{
    public class NotificationsWorker : INotificationsSubscriber, INotificationsNotifier
    {
        private readonly INotificationClients clients;
        private readonly INotificationsManager notifications;

        public NotificationsWorker()
        {
            this.clients = new NotificationClients();
            this.notifications = new NotificationsManager(this.clients);
        }

        public bool AddOrUpdateClient(string clientId, string userId, IClientProxy client)
        {
            return clients.AddOrUpdateClient(clientId, userId, client);
        }

        public string GetUserId(string clientId)
        {
            return clients[clientId]?.UserId;
        }

        public Task<bool> AddClientNotification<T>(string clientIdentificator, Notification<T> notification)
        {
            string clientId = clients.GetClientId(clientIdentificator);

            if (!string.IsNullOrEmpty(clientId))
            {
                return notifications.AddClientNotification(clientId, notification);
            }
            else if (clients[clientIdentificator] != null)
            {
                return notifications.AddClientNotification(clientIdentificator, notification);
            }
            else
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> AddBroadcastNotification<T>(Notification<T> notification)
        {
            return notifications.AddBroadcastNotification(notification);
        }

        public bool RemoveClient(string clientId)
        {
            if (clients.TryRemoveClient(clientId, out var client))
            {
                notifications.RemoveAllClientSubscriptions(client);

                return true;
            }

            return false;
        }

        public bool SubscribeForEvent(string clientId, NotificationTypes type)
        {
            return notifications.SubscribeForEvent(clientId, type);
        }

        public bool UnsubscribeFromEvent(string clientId, NotificationTypes type)
        {
            return notifications.UnsubscribeFromEvent(clientId, type);
        }
    }
}
