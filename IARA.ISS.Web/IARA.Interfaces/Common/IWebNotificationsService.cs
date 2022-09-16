using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.Interfaces.Common
{
    public interface IWebNotificationsService
    {
        Task<bool> NotifyUser<T>(int userId, string templateCode, T model, string notificationUrl = null) where T : class;
        NotificationsDTO GetUserNotifications(int userId, int page, int pageSize);
        bool MarkNotificationAsRead(int userId, int notificationId);
        void MarkNotificationsAsRead(int userId, IEnumerable<int> notificationIDs);
        Task<bool> NotifyAllConnectedUsers<T>(string templateCode, T model, string notificationUrl = null);
        Task<bool> NotifyAllConnectedUsers<T>(string body, string header = null, string notificationUrl = null);
    }
}
