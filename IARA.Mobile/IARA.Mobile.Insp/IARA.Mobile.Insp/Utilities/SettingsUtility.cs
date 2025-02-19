using IARA.Mobile.Insp.Application.Interfaces.Utilities;
using System;
using System.Linq;
using Xamarin.Essentials;

namespace IARA.Mobile.Insp.Utilities
{
    public class SettingsUtility : ISettings
    {
        private const string SharedName = nameof(SettingsUtility);

        public bool SuccessfulLogin
        {
            get => Preferences.Get(nameof(SuccessfulLogin), false, SharedName);
            set => Preferences.Set(nameof(SuccessfulLogin), value, SharedName);
        }

        public bool IsDeviceAllowed
        {
            get => Preferences.Get(nameof(IsDeviceAllowed), false, SharedName);
            set => Preferences.Set(nameof(IsDeviceAllowed), value, SharedName);
        }

        public double FontSize
        {
            get => Preferences.Get(nameof(FontSize), 16d, SharedName);
            set => Preferences.Set(nameof(FontSize), value, SharedName);
        }

        public DateTime? LastInspectionFetchDate
        {
            get
            {
                DateTime value = Preferences.Get(nameof(LastInspectionFetchDate), DateTime.MinValue, SharedName);

                return value == DateTime.MinValue ? null : (DateTime?)value;
            }
            set => Preferences.Set(nameof(LastInspectionFetchDate), value ?? DateTime.MinValue, SharedName);
        }

        public int[] Fleets
        {
            get
            {
                string fleetsStr = Preferences.Get(nameof(Fleets), string.Empty, SharedName);

                if (!string.IsNullOrEmpty(fleetsStr))
                {
                    return fleetsStr.Split(',').Select(int.Parse).ToArray();
                }

                return Array.Empty<int>();
            }
            set => Preferences.Set(nameof(Fleets), string.Join(",", value), SharedName);
        }

        public string LastVersion
        {
            get => Preferences.Get(nameof(LastVersion), VersionTracking.CurrentVersion, SharedName);
            set => Preferences.Set(nameof(LastVersion), value, SharedName);
        }
        public bool IsInspectorAllowed
        {
            get => Preferences.Get(nameof(IsInspectorAllowed), false, SharedName);
            set => Preferences.Set(nameof(IsInspectorAllowed), value, SharedName);
        }

        public int LockInspectionAfterHours
        {
            get => Preferences.Get(nameof(LockInspectionAfterHours), 48, SharedName);
            set => Preferences.Set(nameof(LockInspectionAfterHours), value, SharedName);
        }
        public int LatestSubmissionDateForInspection
        {
            get => Preferences.Get(nameof(LatestSubmissionDateForInspection), 72, SharedName);
            set => Preferences.Set(nameof(LatestSubmissionDateForInspection), value, SharedName);
        }

        public void Clear()
        {
            Preferences.Clear(SharedName);
        }
    }
}
