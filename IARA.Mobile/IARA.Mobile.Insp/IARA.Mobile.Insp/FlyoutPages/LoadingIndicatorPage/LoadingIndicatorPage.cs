using IARA.Mobile.Insp.Helpers;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.LoadingIndicatorPage
{
    public class LoadingIndicatorPage : ContentPage
    {
        private readonly ProgressBar progressBar;

        public LoadingIndicatorPage()
        {
            BackgroundColor = Color.White;

            progressBar = new ProgressBar
            {
                ProgressColor = App.GetResource<Color>("Primary"),
                WidthRequest = 300
            };
            Content = new StackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Image
                    {
                        Source = ImageExtension.Convert("splash_screen_iara"),
                        WidthRequest = 300,
                    },
                    progressBar
                }
            };
        }

        public async void ProgressTo(double progress)
        {
            await progressBar.ProgressTo(progress, 1000, Easing.CubicOut);
        }
    }
}
