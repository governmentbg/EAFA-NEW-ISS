using IARA.Mobile.Application.Interfaces.Utilities;
using Xamarin.Essentials;

namespace IARA.Mobile.Shared.Utilities
{
    public class DbSettingsUtility : IDbSettings
    {
        public int LastVersion
        {
            get => Preferences.Get(nameof(LastVersion), default(int), nameof(DbSettingsUtility));
            set => Preferences.Set(nameof(LastVersion), value, nameof(DbSettingsUtility));
        }

        public void Clear()
        {
            Preferences.Clear(nameof(DbSettingsUtility));
        }
    }
}
