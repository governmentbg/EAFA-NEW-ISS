using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Menu
{
    public class NavigationItemView : Frame
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(NavigationItemView), string.Empty, BindingMode.TwoWay);

        public static readonly BindableProperty IconProperty =
            BindableProperty.Create(nameof(Icon), typeof(string), typeof(NavigationItemView), null, BindingMode.TwoWay);

        public static readonly BindableProperty IsSelectedProperty =
            BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(NavigationItemView), false, BindingMode.TwoWay);

        public NavigationItemView()
        {
            Margin = new Thickness(5, 0);
            Padding = new Thickness(15, 10);
            CornerRadius = 10;
            HasShadow = false;
            HorizontalOptions = LayoutOptions.FillAndExpand;
            BackgroundColor = Color.Transparent;
            Triggers.Add(new DataTrigger(typeof(Frame))
            {
                Binding = new Binding
                {
                    Path = IsSelectedProperty.PropertyName,
                    Source = this
                },
                Setters =
                {
                    new Setter
                    {
                        Property = BackgroundColorProperty,
                        Value = (Color)Xamarin.Forms.Application.Current.Resources["GrayColor"]
                    },
                    new Setter
                    {
                        Property = HasShadowProperty,
                        Value = true
                    }
                },
                Value = true
            });
            Content = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition() { Width = 40 },
                    new ColumnDefinition() { Width = GridLength.Star }
                },
                Children =
                {
                    new Label
                    {
                        BindingContext = this,
                        FontSize = 22,
                        FontFamily = "FA",
                        TextColor = (Color)Xamarin.Forms.Application.Current.Resources["PrimaryDark"],
                        VerticalOptions = LayoutOptions.Center
                    }.Bind(Label.TextProperty, IconProperty.PropertyName),
                    new Label
                    {
                        BindingContext = this,
                        FontSize = 16,
                        LineBreakMode = LineBreakMode.WordWrap,
                        TextColor = (Color)Xamarin.Forms.Application.Current.Resources["Primary"],
                        VerticalOptions = LayoutOptions.Center
                    }.Column(1)
                        .Bind(Label.TextProperty, TextProperty.PropertyName)
                }
            };

            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = CommandBuilder.CreateFrom(() => ItemTapped.Execute(Route))
            });
        }

        public ICommand ItemTapped;

        public string Icon
        {
            get => (string)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        public string Route { get; set; }
    }
}
