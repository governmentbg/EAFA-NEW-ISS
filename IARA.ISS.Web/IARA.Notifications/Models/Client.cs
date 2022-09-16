using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using IARA.Notifications.Enums;
using Microsoft.AspNetCore.SignalR;

namespace IARA.Notifications.Models
{
    internal class Client
    {
        public Client(string clientId, IClientProxy proxy, string userId = null)
        {
            this.ClientId = clientId;
            this.UserId = userId;
            this.Proxy = proxy;
            this.subscriptions = new Dictionary<NotificationTypes, LinkedListNode<string>>();
        }

        private Dictionary<NotificationTypes, LinkedListNode<string>> subscriptions;
        public IClientProxy Proxy { get; set; }
        public string ClientId { get; set; }
        public string UserId { get; set; }

        public bool TryAdd(NotificationTypes type, LinkedListNode<string> subscription)
        {
            if (!subscriptions.ContainsKey(type))
            {
                subscriptions.Add(type, subscription);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Task SendAsync<T>(string method, T message, CancellationToken token)
        {
            return this.Proxy.SendAsync(method, message, token);
        }

        public bool TryRemove(NotificationTypes type, out LinkedListNode<string> subscription)
        {
            if (subscriptions.ContainsKey(type))
            {
                subscription = subscriptions[type];
                subscriptions.Remove(type);
                return true;
            }
            else
            {
                subscription = null;
                return false;
            }
        }

    }
}
