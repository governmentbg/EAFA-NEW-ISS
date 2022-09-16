using System.Threading.Tasks;
using IARA.Notifications.Enums;
using IARA.Notifications.Models;

namespace IARA.Notifications
{
    internal interface INotificationsManager
    {
        Task<bool> AddBroadcastNotification<T>(Notification<T> notification);
        Task<bool> AddClientNotification<T>(string clientId, Notification<T> notification);
        void RemoveAllClientSubscriptions(Client client);
        bool SubscribeForEvent(string clientId, NotificationTypes type);
        bool UnsubscribeFromEvent(string clientId, NotificationTypes type);
    }
}
