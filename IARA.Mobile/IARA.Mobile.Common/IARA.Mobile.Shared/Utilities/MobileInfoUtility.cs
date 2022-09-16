using IARA.Mobile.Application.Interfaces.Utilities;
using Xamarin.Essentials;

namespace IARA.Mobile.Shared.Utilities
{
    public class MobileInfoUtility : IMobileInfo
    {
        public string Info => $"[{DeviceInfo.Idiom}] {{{DeviceInfo.Platform} {DeviceInfo.VersionString}}} {DeviceInfo.Manufacturer} {DeviceInfo.Model} (IARA v{VersionTracking.CurrentVersion})";

        public string Platform => DeviceInfo.Platform.ToString();

        public string CurrentVersion => VersionTracking.CurrentVersion;
    }
}
