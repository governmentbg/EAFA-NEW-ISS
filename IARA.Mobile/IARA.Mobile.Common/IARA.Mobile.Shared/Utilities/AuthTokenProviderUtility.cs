using System;
using IARA.Mobile.Application.Interfaces.Utilities;
using Xamarin.Essentials;

namespace IARA.Mobile.Shared.Utilities
{
    public class AuthTokenProviderUtility : IAuthTokenProvider
    {
        private const string SharedName = nameof(AuthTokenProviderUtility);

        public string Token
        {
            get => Preferences.Get(nameof(Token), string.Empty, SharedName);
            set => Preferences.Set(nameof(Token), value, SharedName);
        }

        public string RefreshToken
        {
            get => Preferences.Get(nameof(RefreshToken), string.Empty, SharedName);
            set => Preferences.Set(nameof(RefreshToken), value, SharedName);
        }

        public DateTime AccessTokenExpiration
        {
            get => Preferences.Get(nameof(AccessTokenExpiration), DateTime.MinValue, SharedName);
            set => Preferences.Set(nameof(AccessTokenExpiration), value, SharedName);
        }

        public bool IsUnregisteredEAuthUser
        {
            get => Preferences.Get(nameof(IsUnregisteredEAuthUser), false, SharedName);
            set => Preferences.Set(nameof(IsUnregisteredEAuthUser), value, SharedName);
        }

        public void Clear()
        {
            Preferences.Clear(nameof(AuthTokenProviderUtility));
        }
    }
}
