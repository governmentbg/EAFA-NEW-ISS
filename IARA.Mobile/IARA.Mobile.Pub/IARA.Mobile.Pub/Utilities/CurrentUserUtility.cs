using IARA.Mobile.Application.Interfaces.Utilities;
using Xamarin.Essentials;

namespace IARA.Mobile.Pub.Utilities
{
    public class CurrentUserUtility : ICurrentUser
    {
        private const string SharedName = nameof(CurrentUserUtility);

        public int Id
        {
            get => Preferences.Get(nameof(Id), default(int), SharedName);
            set => Preferences.Set(nameof(Id), value, SharedName);
        }

        public string EgnLnch
        {
            get => Preferences.Get(nameof(EgnLnch), default(string), SharedName);
            set => Preferences.Set(nameof(EgnLnch), value, SharedName);
        }

        public string[] Permissions
        {
            get
            {
                string permissionString = Preferences.Get(nameof(Permissions), default(string), SharedName);
                return permissionString?.Split(',');
            }
            set => Preferences.Set(nameof(Permissions), value == null ? null : string.Join(",", value), SharedName);
        }

        public string FirstName
        {
            get => Preferences.Get(nameof(FirstName), default(string), SharedName);
            set => Preferences.Set(nameof(FirstName), value, SharedName);
        }

        public string MiddleName
        {
            get => Preferences.Get(nameof(MiddleName), default(string), SharedName);
            set => Preferences.Set(nameof(MiddleName), value, SharedName);
        }

        public string LastName
        {
            get => Preferences.Get(nameof(LastName), default(string), SharedName);
            set => Preferences.Set(nameof(LastName), value, SharedName);
        }

        public bool MustChangePassword
        {
            get => Preferences.Get(nameof(MustChangePassword), default(bool), SharedName);
            set => Preferences.Set(nameof(MustChangePassword), value, SharedName);
        }

        public void Clear()
        {
            Preferences.Clear(SharedName);
        }
    }
}
