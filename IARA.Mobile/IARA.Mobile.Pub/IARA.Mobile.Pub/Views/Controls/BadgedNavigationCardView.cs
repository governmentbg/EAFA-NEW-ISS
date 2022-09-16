using System.Windows.Input;
using TechnoLogica.Xamarin.Controls;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Views.Controls
{
    public class BadgedNavigationCardView : TLFillLayout
    {
        public static readonly BindableProperty IconProperty =
            BindableProperty.Create(nameof(Icon), typeof(string), typeof(BadgedNavigationCardView));

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(BadgedNavigationCardView), string.Empty);

        public static readonly BindableProperty NavigateCommandProperty =
            BindableProperty.Create(nameof(NavigateCommand), typeof(ICommand), typeof(BadgedNavigationCardView));

        public static readonly BindableProperty NavigateCommandParameterProperty =
            BindableProperty.Create(nameof(NavigateCommandParameter), typeof(object), typeof(BadgedNavigationCardView));

        public static readonly BindableProperty BadgeTextProperty =
            BindableProperty.Create(nameof(BadgeText), typeof(string), typeof(BadgedNavigationCardView));

        public BadgedNavigationCardView()
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                TLFlex.SetGrow(this, 1);
                HeightRequest = 100;
            }
            else
            {
                TLFlex.SetGrow(this, 2);
                HeightRequest = 175;
            }

            MeasureMethod = MeasureMethodEnum.BasedOnParent;

            FuncConverter<string, bool> badgeTextVisibilityConverter = new FuncConverter<string, bool>((value) => !string.IsNullOrEmpty(value) && value != "0");

            Children.Add(new NavigationCardView
            {
                BindingContext = this,
            }.Bind(NavigationCardView.IconProperty, IconProperty.PropertyName)
                .Bind(NavigationCardView.TextProperty, TextProperty.PropertyName)
                .Bind(NavigationCardView.NavigateCommandProperty, NavigateCommandProperty.PropertyName)
                .Bind(NavigationCardView.NavigateCommandParameterProperty, NavigateCommandParameterProperty.PropertyName));

            Children.Add(new Grid
            {
                HorizontalOptions = LayoutOptions.End,
                Margin = 5,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = 26 }
                },
                RowDefinitions = new RowDefinitionCollection
                {
                    new RowDefinition { Height = 26 }
                },
                Children =
                {
                    new Frame
                    {
                        BindingContext = this,
                        Padding = 0,
                        CornerRadius = 13,
                        BackgroundColor = Color.FromHex("#58A1E1"),
                        Content = new Label
                        {
                            BindingContext = this,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                            TextColor = Color.White,
                            FontSize = 12,
                        }.Bind(Label.TextProperty, BadgeTextProperty.PropertyName)
                    }.Bind(Frame.IsVisibleProperty,BadgeTextProperty.PropertyName,converter:badgeTextVisibilityConverter)
                }
            });
        }

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

        public ICommand NavigateCommand
        {
            get => (ICommand)GetValue(NavigateCommandProperty);
            set => SetValue(NavigateCommandProperty, value);
        }

        public object NavigateCommandParameter
        {
            get => GetValue(NavigateCommandParameterProperty);
            set => SetValue(NavigateCommandParameterProperty, value);
        }

        public string BadgeText
        {
            get => (string)GetValue(BadgeTextProperty);
            set => SetValue(BadgeTextProperty, value);
        }
    }
}
