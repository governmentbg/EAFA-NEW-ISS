using TechnoLogica.Xamarin.Converters.Base;
using Xamarin.CommunityToolkit.UI.Views;

namespace IARA.Mobile.Shared.Converters
{
    public class BoolToLayoutStateConverter : BaseValueConverter<LayoutState, bool>
    {
        public override LayoutState ConvertTo(bool value)
        {
            return value ? LayoutState.Loading : LayoutState.Custom;
        }
    }
}
