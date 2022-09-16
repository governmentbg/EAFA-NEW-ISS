using System.Threading.Tasks;
using IARA.Notifications.Models;

namespace IARA.Notifications.Interfaces
{
    public interface INotificationsNotifier
    {
        Task<bool> AddClientNotification<T>(string clientIdentificator, Notification<T> notification);
        Task<bool> AddBroadcastNotification<T>(Notification<T> notification);
    }
}
