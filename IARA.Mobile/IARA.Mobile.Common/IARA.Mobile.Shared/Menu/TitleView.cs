using System.Threading.Tasks;
using IARA.Mobile.Application;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Shared.ViewModels.Intrerfaces;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Menu
{
    public class TitleView : ContentView
    {
        private readonly Label _titleLabel;
        private readonly Label _noInternetLabel;
        private readonly Label _infoLabel;
        private readonly IBasePage _page;

        public TitleView(IBasePage page, IConnectivity connectivity)
        {
            if (Device.RuntimePlatform == Device.UWP)
            {
                Margin = new Thickness(10, 0, 0, 0);
            }

            _page = page;
            _titleLabel = new Label
            {
                FontSize = 16,
                TextColor = Color.White,
                VerticalOptions = LayoutOptions.Center,
                Text = page.Title,
                LineBreakMode = LineBreakMode.WordWrap,
                FontAttributes = FontAttributes.Bold
            };
            _noInternetLabel = new Label
            {
                FontFamily = "FA",
                Padding = 10,
                FontSize = Device.RuntimePlatform switch
                {
                    Device.Android => 34,
                    Device.iOS => 24,
                    Device.UWP => 28,
                    _ => 24,
                },
                IsVisible = false,
                Text = IconFont.TriangleExclamation,
                TextColor = (Color)Xamarin.Forms.Application.Current.Resources["ErrorColor"],
                GestureRecognizers =
                {
                    new TapGestureRecognizer()
                    {
                        Command = CommandBuilder.CreateFrom(NoInternetLabelTapped)
                    }
                }
            };
            _infoLabel = new Label
            {
                FontFamily = "FA",
                Padding = 10,
                FontSize = Device.RuntimePlatform switch
                {
                    Device.Android => 34,
                    Device.iOS => 24,
                    Device.UWP => 28,
                    _ => 24,
                },
                Text = IconFont.CircleInfo,
                TextColor = Color.White,
                GestureRecognizers =
                {
                    new TapGestureRecognizer()
                    {
                        Command = CommandBuilder.CreateFrom(InfoLabelTapped)
                    }
                }
            };

            StackLayout titleStack = new StackLayout
            {
                BindingContext = ((BindableObject)page).BindingContext
            };

            for (int i = 0; i < page.TitleViews.Count; i++)
            {
                titleStack.Children.Add(page.TitleViews[i]);
            }

            Content = new Grid
            {
                ColumnSpacing = 0,
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition() { Width = GridLength.Star },
                    new ColumnDefinition() { Width = GridLength.Auto },
                    new ColumnDefinition() { Width = GridLength.Auto },
                },
                Children =
                {
                    _titleLabel,
                    titleStack.Column(1),
                    _noInternetLabel.Column(2),
                    _infoLabel.Column(2)
                }
            };

            if (connectivity != null)
            {
                connectivity.ConnectivityChanged += ConnectivityChanged;
            }

            ConnectivityChanged(connectivity, CommonGlobalVariables.InternetStatus);
        }

        public string TitleText
        {
            get => _titleLabel.Text;
            set => _titleLabel.Text = value;
        }

        private void ConnectivityChanged(object sender, InternetStatus status)
        {
            if (status == InternetStatus.Disconnected)
            {
                _noInternetLabel.IsVisible = true;
                _infoLabel.IsVisible = false;
            }
            else
            {
                _noInternetLabel.IsVisible = false;
                _infoLabel.IsVisible = !string.IsNullOrEmpty(_page.PageInfo);
            }
        }

        private Task InfoLabelTapped()
        {
            return Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                null,
                _page.PageInfo,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Ok"]
            );
        }

        private Task NoInternetLabelTapped()
        {
            string label = Connectivity.NetworkAccess == NetworkAccess.None
                ? "/NoInternetMessage"
                : "/NoServerConnectionMessage";

            return Xamarin.Forms.Application.Current.MainPage.DisplayAlert(
                null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + label],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Ok"]
            );
        }
    }
}