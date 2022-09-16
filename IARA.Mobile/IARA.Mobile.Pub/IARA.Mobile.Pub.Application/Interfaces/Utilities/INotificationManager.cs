using IARA.Mobile.Pub.Domain.Models;
using System.Threading.Tasks;

namespace IARA.Mobile.Pub.Application.Interfaces.Utilities
{
    public interface INotificationManager
    {
        Task<bool> Show(NotificationRequest request);
    }
}
