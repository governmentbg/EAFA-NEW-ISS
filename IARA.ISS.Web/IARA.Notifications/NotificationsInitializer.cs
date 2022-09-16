using IARA.Notifications.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Notifications
{
    public static class NotificationsInitializer
    {
        public static void AddSignalRNotifications(this IServiceCollection services)
        {
            services.AddSingleton<NotificationsWorker>();

            services.AddSingleton<INotificationsSubscriber>((serviceProvider) =>
           {
               return serviceProvider.GetService<NotificationsWorker>();
           });

            services.AddSingleton<INotificationsNotifier>((serviceProvider) =>
           {
               return serviceProvider.GetService<NotificationsWorker>();
           });
        }
    }
}
