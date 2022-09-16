using IARA.Mobile.Application;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Shared.Popups;
using IARA.Mobile.Shared.ResourceTranslator;
using IARA.Mobile.Shared.ViewModels.Base;
using IARA.Mobile.Shared.ViewModels.Intrerfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Shared.Views
{
    public abstract class MainPage<TViewModel> : ContentPage, IBasePage
        where TViewModel : class, IPageViewModel
    {
        public static readonly BindableProperty PageInfoProperty =
            BindableProperty.Create(nameof(PageInfo), typeof(string), typeof(MainPage<>));

        private bool appearingCalled;

        protected MainPage()
        {
            ViewModel = DependencyService.Resolve<TViewModel>();

            if (ViewModel == null)
            {
                throw new ArgumentNullException(nameof(ViewModel), "DependencyService returned null ViewModel of type " + typeof(TViewModel).Name + " inside of MainPage.");
            }

            BindingContext = ViewModel;

            Disappearing += delegate
            {
                if (appearingCalled)
                {
                    ViewModel.OnDisappearing();
                }
            };

            Appearing += OnPageAppearing;

            Translator.Current.Add(ViewModel.GetPageResources(out GroupResourceEnum[] filtered));
            AddedResources = filtered;
        }

        public GroupResourceEnum[] AddedResources { get; }

        protected TViewModel ViewModel { get; }

        protected abstract string PageName { get; }

        public string PageInfo
        {
            get => (string)GetValue(PageInfoProperty);
            set => SetValue(PageInfoProperty, value);
        }

        public List<View> TitleViews { get; } = new List<View>();

        public GroupResourceEnum[] GetPageIndexes()
        {
            return ViewModel.GetPageIndexes();
        }

        private void OnPageAppearing(object sender, EventArgs e)
        {
            IPageVersion version = DependencyService.Resolve<IPageVersion>();

            if (version.IsPageAllowed(PageName, DeviceInfo.Platform.ToString()))
            {
                ViewModel.OnAppearing();
                if (!appearingCalled)
                {
                    appearingCalled = true;
                    ViewModel.Initialize(this).SafeFireAndForget();
                }
            }
            else
            {
                const string group = nameof(GroupResourceEnum.Common);
                Content = new StackLayout
                {
                    Margin = 50,
                    Spacing = 0,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new Image
                        {
                            Source = NewMajorVersionPopup.ImageSourceConvert("iara_logo"),
                            HeightRequest = 200,
                            WidthRequest = 200,
                            HorizontalOptions = LayoutOptions.Center
                        },
                        new Label
                        {
                            FontSize = 24,
                            Padding = 5,
                            FontAttributes = FontAttributes.Bold,
                            HorizontalOptions = LayoutOptions.Center,
                        }.BindTranslation(Label.TextProperty, "NewMajorVersionTitle", group),
                        new Label
                        {
                            Margin = new Thickness(20, 10),
                            LineBreakMode = LineBreakMode.WordWrap,
                            HorizontalTextAlignment = TextAlignment.Center,
                        }.BindTranslation(Label.TextProperty, "NewPageMajorVersion", group),
                        new Button
                        {
                            Margin = new Thickness(20, 10),
                            Command = CommandBuilder.CreateFrom(OnUpdate)
                        }.BindTranslation(Label.TextProperty, "UpdateApplication", group),
                     }
                };
            }
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

    }
}
