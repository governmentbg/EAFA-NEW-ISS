using Foundation;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UserNotifications;

namespace IARA.Mobile.Pub.iOS
{
    public static class NotificationCenter
    {
        public static async Task<bool> AskPermissionAsync()
        {
            try
            {
                UNNotificationSettings settings = await UNUserNotificationCenter.Current
                    .GetNotificationSettingsAsync()
                    .ConfigureAwait(false);

                if (settings.AlertSetting == UNNotificationSetting.Enabled)
                {
                    return true;
                }

                // Ask the user for permission to show notifications on iOS 10.0+
                (bool alertsAllowed, NSError error) = await UNUserNotificationCenter.Current
                    .RequestAuthorizationAsync(
                        UNAuthorizationOptions.Alert |
                        UNAuthorizationOptions.Badge |
                        UNAuthorizationOptions.Sound)
                    .ConfigureAwait(false);

                Debug.WriteLine(error?.LocalizedDescription);
                return alertsAllowed;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
        }
    }
}
