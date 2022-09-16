using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Views
{
    [ContentProperty(nameof(InnerContent))]
    public class HomeFrameView : ContentView
    {
        public static readonly BindableProperty InnerContentProperty =
            BindableProperty.Create(nameof(InnerContent), typeof(View), typeof(HomeFrameView));

        public HomeFrameView()
        {
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;

            if (Device.Idiom == TargetIdiom.Phone)
            {
                Padding = 20;
                this.Bind(ContentProperty, InnerContentProperty.PropertyName, source: this);
            }
            else
            {
                Padding = 10;

                Content = new Frame
                {
                    Padding = 30,
                    WidthRequest = 400
                }.Bind(Frame.ContentProperty, InnerContentProperty.PropertyName, source: this);
            }
        }

        public View InnerContent
        {
            get => (View)GetValue(InnerContentProperty);
            set => SetValue(InnerContentProperty, value);
        }
    }
}
