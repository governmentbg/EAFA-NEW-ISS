using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Foundation;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Domain.Models;
using UIKit;
using UserNotifications;

namespace IARA.Mobile.Pub.iOS.Utilities
{
    public class IosNotification : INotificationManager
    {
        public async Task<bool> Show(NotificationRequest request)
        {
            try
            {
                if (!UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                {
                    return false;
                }

                bool allowed = await NotificationCenter.AskPermissionAsync().ConfigureAwait(false);
                if (!allowed)
                {
                    return false;
                }

                NSMutableDictionary userInfoDictionary = new NSMutableDictionary();

                if (request.Data?.Count > 0)
                {
                    foreach (KeyValuePair<string, object> pair in request.Data)
                    {
                        userInfoDictionary.SetValueForKey(new NSString(pair.Value.ToString()), new NSString(pair.Key));
                    }
                }

                userInfoDictionary.SetValueForKey(new NSString("True"), new NSString(Notifications.IS_LOCAL));

                using (UNMutableNotificationContent content = new UNMutableNotificationContent
                {
                    Title = request.Title,
                    Subtitle = "",
                    Body = request.Body,
                    Badge = 1,
                    UserInfo = userInfoDictionary,
                    Sound = UNNotificationSound.Default,
                })
                {
                    if (!UIDevice.CurrentDevice.CheckSystemVersion(12, 0))
                    {
                        content.SetValueForKey(NSObject.FromObject(true), new NSString("shouldAlwaysAlertWhileAppIsForeground"));
                    }

                    UNNotificationRequest nativeRequest = UNNotificationRequest.FromIdentifier("0", content, null);

                    await UNUserNotificationCenter.Current.AddNotificationRequestAsync(nativeRequest)
                        .ConfigureAwait(false);

                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}

