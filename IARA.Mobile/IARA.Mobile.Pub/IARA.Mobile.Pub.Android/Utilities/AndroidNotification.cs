using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Domain.Models;
using Plugin.FirebasePushNotification;

namespace IARA.Mobile.Pub.Droid.Utilities
{
    public class AndroidNotification : INotificationManager
    {
        private readonly IExceptionHandler _handler;

        public AndroidNotification(IExceptionHandler handler)
        {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public Task<bool> Show(NotificationRequest request)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.IceCreamSandwich)
            {
                return Task.FromResult(false);
            }

            Notification.Builder builder;

            try
            {
                builder = new Notification.Builder(MainActivity.Current, FirebasePushNotificationManager.DefaultNotificationChannelId)
                    .SetContentTitle(request.Title)
                    .SetContentText(request.Body)
                    .SetSmallIcon(Resource.Mipmap.icon)
                    .SetAutoCancel(true);
            }
            catch (Exception ex)
            {
                _handler.HandleException(ex);
                return Task.FromResult(false);
            }

            if (!string.IsNullOrEmpty(request.Group))
            {
                builder.SetGroup(request.Group);
            }

            Intent notificationIntent = MainActivity.Current.PackageManager?.GetLaunchIntentForPackage(MainActivity.Current.PackageName ?? string.Empty)
                .SetFlags(ActivityFlags.SingleTop);

            if (request.Data?.Count > 0)
            {
                foreach (KeyValuePair<string, object> pair in request.Data)
                {
                    notificationIntent.PutExtra(pair.Key, pair.Value.ToString());
                }
            }

            PendingIntent pendingIntent = PendingIntent.GetActivity(MainActivity.Current, new Random().Next(), notificationIntent, PendingIntentFlags.Immutable);
            builder.SetContentIntent(pendingIntent);

            Notification notification = builder.Build();

            NotificationManager manager = (NotificationManager)MainActivity.Current.GetSystemService(Context.NotificationService);

            manager.Notify(new Random().Next(), notification);

            builder.Dispose();
            return Task.FromResult(true);
        }
    }
}
