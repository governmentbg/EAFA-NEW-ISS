using IARA.Mobile.Application;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Shared.Views;
using Rg.Plugins.Popup.Pages;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Commands;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Popups
{
    public class NewMajorVersionPopup : PopupPage
    {
        public NewMajorVersionPopup()
        {
            Content = new Frame
            {
                HasShadow = false,
                Margin = 50,
                Padding = 0,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Content = new StackLayout
                {
                    Spacing = 0,
                    Children =
                    {
                        new Image
                        {
                            Source = ImageSourceConvert("iara_logo"),
                            HeightRequest = 150,
                            WidthRequest = 150,
                            Margin = new Thickness(0, 15, 0, 0),
                            HorizontalOptions = LayoutOptions.Center
                        },
                        new Label
                        {
                            FontSize = 24,
                            Padding = 5,
                            FontAttributes = FontAttributes.Bold,
                            Text = "Внимание!",
                            HorizontalOptions = LayoutOptions.Center,
                        },
                        new Label
                        {
                            Margin = new Thickness(20, 10),
                            Text = "Използвате стара версия на приложението. Необходимо е да актуализирате приложението за да продължите да го използвате.",
                            LineBreakMode = LineBreakMode.WordWrap,
                            HorizontalTextAlignment = TextAlignment.Center,
                        },
                        new TLButton
                        {
                            Margin = new Thickness(20, 10),
                            Text = "Актуализирай приложението",
                            Command = CommandBuilder.CreateFrom(OnUpdate)
                        }
                    }
                }
            };
        }

        private Task OnUpdate()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                return Browser.OpenAsync($"https://play.google.com/store/apps/details?id={AppInfo.PackageName}");
            }
            else if (Device.RuntimePlatform == Device.UWP)
            {
                IServerUrl serverUrl = DependencyService.Resolve<IServerUrl>();
                if (serverUrl.Environment == Environments.PRODUCTION)
                {
                    return Browser.OpenAsync($"https://www.microsoft.com/bg-bg/p/iara-inspectors/9p1b2r8stt4l");
                }
                else if (serverUrl.Environment == Environments.STAGING)
                {
                    return Browser.OpenAsync($"https://www.microsoft.com/bg-bg/p/iara-inspectors/9PFGKH1JMSSL");
                }

                return Task.CompletedTask;
            }
            else
            {
                return Browser.OpenAsync($"https://apps.apple.com/us/app/%D0%B8%D0%B0%D1%80%D0%B0-%D0%BB%D1%8E%D0%B1%D0%B8%D1%82%D0%B5%D0%BB%D1%81%D0%BA%D0%B8-%D1%80%D0%B8%D0%B1%D0%BE%D0%BB%D0%BE%D0%B2/id1594577744");
            }
        }

        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return true;
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return false;
        }

        public static FileImageSource ImageSourceConvert(string path)
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                path = System.IO.Path.Combine($"Images/{path}.png");
            }

            return (FileImageSource)ImageSource.FromFile(path);
        }
    }
}
