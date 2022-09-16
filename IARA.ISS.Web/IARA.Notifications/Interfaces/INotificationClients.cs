using IARA.Notifications.Models;
using Microsoft.AspNetCore.SignalR;

namespace IARA.Notifications
{
    internal interface INotificationClients
    {
        Client this[string clientId] { get; }
        string GetClientId(string userId);
        bool AddOrUpdateClient(string clientId, string userId, IClientProxy client);
        bool TryRemoveClient(string clientId, out Client client);
    }
}
