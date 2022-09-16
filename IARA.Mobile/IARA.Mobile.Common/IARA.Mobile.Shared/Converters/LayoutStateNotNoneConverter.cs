using TechnoLogica.Xamarin.Converters.Base;
using Xamarin.CommunityToolkit.UI.Views;

namespace IARA.Mobile.Shared.Converters
{
    public class LayoutStateNotNoneConverter : BaseValueConverter<bool, LayoutState>
    {
        public override bool ConvertTo(LayoutState value)
        {
            return value != LayoutState.None;
        }
    }
}
