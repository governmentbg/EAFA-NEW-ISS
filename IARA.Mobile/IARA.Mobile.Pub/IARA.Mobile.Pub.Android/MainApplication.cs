using Android.App;
using Android.OS;
using Android.Runtime;
using Plugin.FirebasePushNotification;
using System;

namespace IARA.Mobile.Pub.Droid
{
    [Application(UsesCleartextTraffic = true)]
    public class MainApplication : Android.App.Application
    {
        public MainApplication(IntPtr handle, JniHandleOwnership transer)
          : base(handle, transer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            //Set the default notification channel for your app when running Android Oreo
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                //Change for your default notification channel id here
                FirebasePushNotificationManager.DefaultNotificationChannelId = "DefaultChannel";

                //Change for your default notification channel name here
                FirebasePushNotificationManager.DefaultNotificationChannelName = "General";
            }

            //If debug you should reset the token each time.
#if DEBUG
            FirebasePushNotificationManager.Initialize(this, new NotificationUserCategory[] { }, true);
#else
            FirebasePushNotificationManager.Initialize(this, new NotificationUserCategory[] { }, false);
#endif
        }
    }
}