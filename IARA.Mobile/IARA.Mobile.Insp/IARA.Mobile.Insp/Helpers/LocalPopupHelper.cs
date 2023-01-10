using System.Threading.Tasks;
using IARA.Mobile.Insp.FlyoutPages.AddInspectionsPage;
using Rg.Plugins.Popup.Services;

namespace IARA.Mobile.Insp.Helpers
{
    public static class LocalPopupHelper
    {
        public static Task OpenAddInspectionsDrawer()
        {
            return PopupNavigation.Instance.PushAsync(new AddInspectionsPopup());
        }
    }
}
