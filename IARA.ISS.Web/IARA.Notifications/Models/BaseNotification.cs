using IARA.Notifications.Enums;

namespace IARA.Notifications.Models
{
    public abstract class BaseNotification
    {
        public NotificationTypes Type { get; set; }
        public virtual object Message { get; protected set; }
    }
}
