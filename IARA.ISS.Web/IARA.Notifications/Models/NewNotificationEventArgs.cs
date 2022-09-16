using System;
using IARA.Notifications.Enums;

namespace IARA.Notifications.Models
{
    public class NewNotificationEventArgs : EventArgs
    {
        public NewNotificationEventArgs(NotificationTypes type, BaseNotification notification)
        {
            this.Type = type;
            this.Notification = notification;
        }

        public readonly NotificationTypes Type;
        public readonly BaseNotification Notification;
    }
}
