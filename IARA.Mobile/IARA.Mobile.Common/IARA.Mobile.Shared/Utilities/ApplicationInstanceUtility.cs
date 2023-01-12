using System;
using IARA.Mobile.Application.Interfaces.Utilities;
using Xamarin.Essentials;

namespace IARA.Mobile.Shared.Utilities
{
    public class ApplicationInstanceUtility : IApplicationInstance
    {
        private const string SharedName = nameof(ApplicationInstanceUtility);

        public string Id
        {
            get
            {
                string id = Preferences.Get(nameof(Id), string.Empty, SharedName);
                if (string.IsNullOrEmpty(id))
                {
                    id = Guid.NewGuid().ToString();
                    Preferences.Set(nameof(Id), id, SharedName);
                }
                return id;
            }
        }
    }
}
