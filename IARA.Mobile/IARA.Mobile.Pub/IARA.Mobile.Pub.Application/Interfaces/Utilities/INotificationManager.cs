using System.Threading.Tasks;
using IARA.Mobile.Pub.Domain.Models;

namespace IARA.Mobile.Pub.Application.Interfaces.Utilities
{
    public interface INotificationManager
    {
        Task<bool> Show(NotificationRequest request);
    }
}
