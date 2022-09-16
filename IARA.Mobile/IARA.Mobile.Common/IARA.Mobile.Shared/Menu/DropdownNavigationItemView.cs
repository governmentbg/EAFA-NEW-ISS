using IARA.Mobile.Shared.Converters;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Menu
{
    public class DropdownNavigationItemView : Expander
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(NavigationItemView), string.Empty, BindingMode.TwoWay);

        public static readonly BindableProperty IconProperty =
            BindableProperty.Create(nameof(Icon), typeof(string), typeof(NavigationItemView), null, BindingMode.TwoWay);

        private readonly NavigationItemView _navItem;

        public DropdownNavigationItemView()
        {
            CollapseAnimationLength = 0;
            ExpandAnimationLength = 0;

            _navItem = new NavigationItemView();
            _navItem.GestureRecognizers.Clear();
            _navItem.SetBinding(NavigationItemView.TextProperty, new Binding
            {
                Path = nameof(Text),
                Source = this
            });
            _navItem.SetBinding(NavigationItemView.IconProperty, new Binding
            {
                Path = nameof(Icon),
                Source = this
            });

            Label label = new Label
            {
                FontSize = 20,
                FontFamily = "FA",
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.Black,
            };
            label.SetBinding(Label.TextProperty, new Binding
            {
                Converter = new AccordionImageConverter(),
                Path = nameof(IsExpanded),
                Source = this
            });

            Header = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                Children =
                {
                    _navItem,
                    label
                }
            };
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
    }
}
