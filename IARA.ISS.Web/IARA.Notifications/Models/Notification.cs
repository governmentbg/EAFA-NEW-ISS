using IARA.Notifications.Enums;

namespace IARA.Notifications.Models
{
    public class Notification<T> : BaseNotification
    {
        public Notification(NotificationTypes type)
        {
            Type = type;
        }

        public new T Message
        {
            get
            {
                return (T)base.Message;
            }
            set
            {
                base.Message = value;
            }
        }
    }
}
