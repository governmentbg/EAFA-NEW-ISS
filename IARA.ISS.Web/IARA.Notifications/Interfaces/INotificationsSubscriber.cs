using IARA.Notifications.Enums;
using Microsoft.AspNetCore.SignalR;

namespace IARA.Notifications.Interfaces
{
    public interface INotificationsSubscriber
    {
        bool AddOrUpdateClient(string clientId, string userId, IClientProxy client);
        string GetUserId(string clientId);
        bool RemoveClient(string clientId);
        bool SubscribeForEvent(string clientId, NotificationTypes type);
        bool UnsubscribeFromEvent(string clientId, NotificationTypes type);
    }
}
