using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Views.FlyoutPages;

namespace IARA.Mobile.Pub.Utilities
{
    public class CommonNavigatorUtility : ICommonNavigator
    {
        /// <summary>
        /// Changes the current page with the Login page
        /// </summary>
        public void ToLogin()
        {
            App.Current.SetMainPage(new LoginPage());
        }
    }
}
