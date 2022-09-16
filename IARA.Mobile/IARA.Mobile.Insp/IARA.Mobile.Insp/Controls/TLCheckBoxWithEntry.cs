using TechnoLogica.Xamarin.Controls;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Controls
{
    public class TLCheckBoxWithEntry : TLEntry
    {
        public static readonly BindableProperty IsCheckedProperty =
            BindableProperty.Create(nameof(IsChecked), typeof(bool), typeof(TLCheckBoxWithEntry), false, BindingMode.TwoWay);

        public TLCheckBoxWithEntry()
        {
            TitleView = new TLCheckView
            {
                BindingContext = this
            }.Bind(TLCheckView.IsCheckedProperty, IsCheckedProperty.PropertyName)
                .Bind(TLCheckView.TextProperty, TitleProperty.PropertyName)
                .Bind(TLCheckView.IsEnabledProperty, IsEnabledProperty.PropertyName);

            FrameWrapper.Bind(Frame.IsVisibleProperty, IsCheckedProperty.PropertyName);
        }

        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
    }
}
