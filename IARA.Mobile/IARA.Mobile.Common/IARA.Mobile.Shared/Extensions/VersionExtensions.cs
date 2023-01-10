using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Extensions
{
    public static class VersionExtensions
    {
        public static int GetBuilderNumber()
        {
            string currentVersion = VersionTracking.CurrentVersion;
            if (currentVersion == "DEBUG")
            {
                return int.MaxValue;
            }

            if (Device.RuntimePlatform == Device.UWP)
            {
                return int.Parse(VersionTracking.CurrentBuild);
            }

            int dotIndex = currentVersion.LastIndexOf('.') + 1;
            string buildNumber = currentVersion.Substring(dotIndex);

            return int.Parse(buildNumber);
        }
    }
}
