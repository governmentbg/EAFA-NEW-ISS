using System.Threading.Tasks;
using IARA.Mobile.Shared.Popups;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace IARA.Mobile.Shared.Helpers
{
    public static class PopupHelper
    {
        public static Task<TReturn> ShowSelectDialog<TReturn, TView>(string title, TView view)
             where TView : View, IPopupSelect<TReturn>
        {
            return TLPopupHelper.ShowSelectDialog<TReturn, TView>(
                title,
                view,
                (Color)Xamarin.Forms.Application.Current.Resources["BackgroundColor"],
                (Color)Xamarin.Forms.Application.Current.Resources["Primary"],
                Color.White,
                Color.White
            );
        }

        public static async Task<MapClickedEventArgs> OpenLocationPicker()
        {
            await TLLoadingHelper.ShowFullLoadingScreen();
            Location geolocation = await Geolocation.GetLocationAsync();

            if (geolocation == null)
            {
                await TLLoadingHelper.HideFullLoadingScreen();
                return null;
            }

            return await TLPopupHelper.OpenPopup<MapClickedEventArgs, SelectLocationOnMapPopup>(new SelectLocationOnMapPopup(geolocation), hideLoading: true);
        }
    }
}
