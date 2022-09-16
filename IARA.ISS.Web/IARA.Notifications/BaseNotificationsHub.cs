using System;
using System.Security.Claims;
using System.Threading.Tasks;
using IARA.Notifications.Enums;
using IARA.Notifications.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace IARA.Notifications
{
    public abstract class BaseNotificationsHub : Hub
    {
        protected readonly INotificationsSubscriber subscriber;
        protected BaseNotificationsHub(INotificationsSubscriber subscriber)
        {
            this.subscriber = subscriber;
        }

        public override Task OnConnectedAsync()
        {
            subscriber.AddOrUpdateClient(this.Context.ConnectionId, GetUserIdentifier(this.Context.User), this.Clients.Caller);

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            string connectionId = this.Context?.ConnectionId;
            if (!string.IsNullOrEmpty(connectionId))
            {
                subscriber.RemoveClient(connectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        public abstract bool UpdateUserData(string token);

        protected virtual bool UpdateUserData(ClaimsPrincipal user)
        {
            return subscriber.AddOrUpdateClient(this.Context.ConnectionId, GetUserIdentifier(user), this.Clients.Caller);
        }

        public bool SubscribeFor(NotificationTypes type)
        {
            string connectionId = this.Context.ConnectionId;
            return subscriber.SubscribeForEvent(connectionId, type);
        }

        public bool UnsubscribeFrom(NotificationTypes type)
        {
            string connectionId = this.Context.ConnectionId;
            return subscriber.UnsubscribeFromEvent(connectionId, type);
        }

        protected abstract string GetUserIdentifier(ClaimsPrincipal user);
    }
}
