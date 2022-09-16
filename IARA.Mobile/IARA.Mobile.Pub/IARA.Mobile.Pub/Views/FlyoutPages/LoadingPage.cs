using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms;

namespace IARA.Mobile.Pub.Views.FlyoutPages
{
    public class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            BackgroundColor = Color.White;

            Content = new TLFillLayout
            {
                Children =
                {
                    new Image
                    {
                        Source = "splash_screen_iara",
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center
                    },
                    new ActivityIndicator
                    {
                        IsRunning = true,
                        Color = App.GetResource<Color>("Primary"),
                        WidthRequest = 40,
                        HeightRequest = 40,
                        VerticalOptions= LayoutOptions.End,
                        HorizontalOptions= LayoutOptions.Center,
                        Margin = new Thickness(0, 0, 0, 20),
                    }
                }
            };
        }
    }
}
