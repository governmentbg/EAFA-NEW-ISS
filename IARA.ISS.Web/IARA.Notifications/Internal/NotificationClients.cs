using System.Collections.Concurrent;
using IARA.Notifications.Models;
using Microsoft.AspNetCore.SignalR;

namespace IARA.Notifications
{
    internal class NotificationClients : INotificationClients
    {
        private readonly ConcurrentDictionary<string, Client> clients;
        private readonly ConcurrentDictionary<string, string> clientToUserIdentificators;
        private readonly ConcurrentDictionary<string, string> userToClientIdentificators;

        public NotificationClients()
        {
            clients = new ConcurrentDictionary<string, Client>();
            clientToUserIdentificators = new ConcurrentDictionary<string, string>();
            userToClientIdentificators = new ConcurrentDictionary<string, string>();
        }

        public Client this[string userIdentificator]
        {
            get
            {
                string clientId = this.GetClientId(userIdentificator);

                if (clientId != null)
                {
                    return clients[clientId];
                }
                else
                {
                    if (clients.ContainsKey(userIdentificator))
                    {
                        return clients[userIdentificator];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public bool AddOrUpdateClient(string clientId, string userId, IClientProxy client)
        {
            clients.AddOrUpdate(clientId, new Client(clientId, client, userId), (key, value) =>
             {
                 value.Proxy = client;
                 value.UserId = userId;
                 return value;
             });

            if (!string.IsNullOrEmpty(userId))
            {
                clientToUserIdentificators.AddOrUpdate(clientId, userId, (key, value) => { return userId; });
                userToClientIdentificators.AddOrUpdate(userId, clientId, (key, value) => { return clientId; });
            }

            return true;
        }

        public string GetClientId(string userId)
        {
            if (userToClientIdentificators.ContainsKey(userId))
            {
                return userToClientIdentificators[userId];
            }

            return null;
        }

        public bool TryRemoveClient(string clientId, out Client client)
        {
            if (clients.TryRemove(clientId, out client) && clientToUserIdentificators.TryRemove(clientId, out _))
            {
                return true;
            }

            return false;
        }

    }
}
