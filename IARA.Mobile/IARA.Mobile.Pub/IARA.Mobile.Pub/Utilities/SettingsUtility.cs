using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using Xamarin.Essentials;

namespace IARA.Mobile.Pub.Utilities
{
    public class SettingsUtility : ISettings
    {
        private const string SharedName = nameof(SettingsUtility);

        public bool SuccessfulLogin
        {
            get => Preferences.Get(nameof(SuccessfulLogin), false, SharedName);
            set => Preferences.Set(nameof(SuccessfulLogin), value, SharedName);
        }

        public ResourceLanguageEnum CurrentResourceLanguage
        {
            get
            {
                int enumInt = Preferences.Get(nameof(CurrentResourceLanguage), default(int), SharedName);
                return (ResourceLanguageEnum)enumInt;
            }
            set => Preferences.Set(nameof(CurrentResourceLanguage), (int)value, SharedName);
        }

        public void Clear()
        {
            Preferences.Clear(SharedName);
        }
    }
}
