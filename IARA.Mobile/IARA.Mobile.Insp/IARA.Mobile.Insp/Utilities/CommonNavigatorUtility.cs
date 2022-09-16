using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.FlyoutPages.LoginPage;

namespace IARA.Mobile.Insp.Utilities
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
