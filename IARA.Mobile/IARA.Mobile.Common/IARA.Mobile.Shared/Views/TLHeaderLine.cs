using TechnoLogica.Xamarin.Helpers;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Views
{
    public class TLHeaderLine : Grid
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(TLHeaderLine), string.Empty);

        public static readonly BindableProperty ImageColorProperty =
            BindableProperty.Create(nameof(ImageColor), typeof(Color), typeof(TLHeaderLine), Color.Black);

        public static readonly BindableProperty ImageGlyphProperty =
            BindableProperty.Create(nameof(ImageGlyph), typeof(string), typeof(TLHeaderLine), IconFont.User);

        public TLHeaderLine()
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = 45 },
                new ColumnDefinition { Width = GridLength.Star }
            };
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = 1 }
            };

            Children.Add(new Label
            {
                BindingContext = this,
                Margin = new Thickness(5, 0, 0, 0),
                FontFamily = "FA",
                FontSize = 24,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
            }.Bind(Label.TextColorProperty, ImageColorProperty.PropertyName)
                .Bind(Label.TextProperty, ImageGlyphProperty.PropertyName));

            Children.Add(new Label
            {
                BindingContext = this,
                LineBreakMode = LineBreakMode.WordWrap,
                FontSize = 18,
                VerticalOptions = LayoutOptions.Center,
                Triggers =
                {
                    new DataTrigger(typeof(Label))
                    {
                        BindingContext = this,
                        Binding = new Binding(IsEnabledProperty.PropertyName),
                        Value = false,
                    }
                }
            }.Bind(Label.TextProperty, TextProperty.PropertyName)
                .Column(1));

            Children.Add(new BoxView
            {
                HeightRequest = 1,
                BackgroundColor = Color.FromHex("#3c424d")
            }.Row(1).ColumnSpan(2));
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public Color ImageColor
        {
            get => (Color)GetValue(ImageColorProperty);
            set => SetValue(ImageColorProperty, value);
        }

        public string ImageGlyph
        {
            get => (string)GetValue(ImageGlyphProperty);
            set => SetValue(ImageGlyphProperty, value);
        }
    }
}
