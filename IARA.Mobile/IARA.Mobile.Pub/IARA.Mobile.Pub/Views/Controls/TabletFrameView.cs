using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Views.Controls
{
    [ContentProperty(nameof(InnerContent))]
    public class TabletFrameView : ContentView
    {
        public static readonly BindableProperty InnerContentProperty =
            BindableProperty.Create(nameof(InnerContent), typeof(View), typeof(TabletFrameView));

        public TabletFrameView()
        {
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.Start;

            if (Device.Idiom == TargetIdiom.Tablet)
            {
                Padding = 10;

                Content = new Frame
                {
                    Padding = 30,
                    WidthRequest = 400
                }.Bind(Frame.ContentProperty, InnerContentProperty.PropertyName, source: this);
            }
            else
            {
                this.Bind(ContentProperty, InnerContentProperty.PropertyName, source: this);
            }
        }

        public View InnerContent
        {
            get => (View)GetValue(InnerContentProperty);
            set => SetValue(InnerContentProperty, value);
        }
    }
}
