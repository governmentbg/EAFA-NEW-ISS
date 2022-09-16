using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Views.Controls
{
    public class NewsLayout : ContentView
    {
        public static readonly BindableProperty InnerContentProperty =
               BindableProperty.Create(nameof(InnerContent), typeof(View), typeof(NewsLayout));

        public NewsLayout()
        {
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Center;

            if (Device.Idiom == TargetIdiom.Phone)
            {
                Padding = 10;
                this.Bind(ContentProperty, InnerContentProperty.PropertyName, source: this);
            }
            else
            {
                Padding = 10;

                Content = new Frame
                {
                    Padding = 30,
                    WidthRequest = 640
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