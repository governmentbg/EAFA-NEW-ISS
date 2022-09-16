using System.Windows.Input;
using TechnoLogica.Xamarin.Helpers;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Controls
{
    public class FloatingAddButtonView : Frame
    {
        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(FloatingAddButtonView));

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(FloatingAddButtonView));

        public FloatingAddButtonView()
        {
            WidthRequest = 60;
            HeightRequest = 60;
            CornerRadius = 30;
            VerticalOptions = LayoutOptions.End;
            HorizontalOptions = LayoutOptions.End;
            Margin = 10;
            Padding = 0;

            Content = new ImageButton
            {
                Padding = 15,
                Source = new FontImageSource
                {
                    FontFamily = "FA",
                    Glyph = IconFont.Plus,
                    Color = Color.White
                }
            }.Bind(ImageButton.CommandProperty, CommandProperty.PropertyName, source: this)
                .Bind(ImageButton.CommandParameterProperty, CommandParameterProperty.PropertyName, source: this);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }
    }
}
