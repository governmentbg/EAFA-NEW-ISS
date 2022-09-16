namespace IARA.Notifications.Models
{
    public class ClientNotification
    {
        public ClientNotification(string clientId, BaseNotification notification)
        {
            this.ClientId = clientId;
            this.Notification = notification;
        }

        public string ClientId { get; set; }
        public BaseNotification Notification { get; set; }
    }
}
