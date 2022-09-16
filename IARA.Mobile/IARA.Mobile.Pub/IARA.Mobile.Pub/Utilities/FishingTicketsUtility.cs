using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using Xamarin.Essentials;

namespace IARA.Mobile.Pub.Utilities
{
    public class FishingTicketsUtility : IFishingTicketsSettings
    {
        private const string SharedName = nameof(FishingTicketsUtility);

        public int AllowedUnder14TicketsCount
        {
            get => Preferences.Get(nameof(AllowedUnder14TicketsCount), default(int), SharedName);
            set => Preferences.Set(nameof(AllowedUnder14TicketsCount), value, SharedName);
        }

        public void Clear()
        {
            Preferences.Clear(SharedName);
        }
    }
}
