using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Application.Interfaces.Transactions;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.Application.DTObjects.News;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Domain.Models;
using IARA.Mobile.Pub.Extensions;
using IARA.Mobile.Pub.ViewModels.FlyoutPages.News;
using IARA.Mobile.Pub.Views.FlyoutPages;
using IARA.Mobile.Pub.Views.FlyoutPages.News;
using IARA.Mobile.Shared.Menu;
using IARA.Mobile.Shared.ResourceTranslator;
using IARA.Mobile.Shared.Views;
using Plugin.FirebasePushNotification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Controls;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Pub
{
    public partial class App : Xamarin.Forms.Application
    {
        public App()
        {
            InitializeComponent();
            UserAppTheme = OSAppTheme.Light;
            Current = this;
            MainPage = new LoadingPage();
        }

        public static new App Current { get; private set; }
        public bool IsAppInTray { get; private set; }

        public void SetMainPage(Page page)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                while (true)
                {
                    try
                    {
                        MainPage = page;
                        return;
                    }
                    catch
                    {
                        // There is a very slight chance this will fail (problem comes from Xamarin itself)
                    }
                }
            });
        }

        public static T GetResource<T>(string index)
        {
            return (T)Current.Resources[index];
        }

        public void LoadInitialResources()
        {
            GroupResourceEnum[] safeResources = new[] { GroupResourceEnum.Common, GroupResourceEnum.Menu, GroupResourceEnum.Validation };

            IReadOnlyDictionary<GroupResourceEnum, IReadOnlyDictionary<string, string>> resources =
                DependencyService.Resolve<ITranslationTransaction>()
                    .GetPagesTranslations(safeResources);

            Translator.Current.SafeResources(safeResources);
            Translator.Current.Add(resources);

            // Set resources after load
            SetStyles();

            Translator.Current.PropertyChanged += OnTranslatorPropertyChanged;
        }

        protected override void OnResume()
        {
            IsAppInTray = false;
        }

        protected override void OnSleep()
        {
            IsAppInTray = true;
        }

        protected override void OnStart()
        {
            CrossFirebasePushNotification.Current.OnTokenRefresh += async (s, p) =>
            {
                while (!CommonGlobalVariables.FinishedSetup)
                {
                    await Task.Delay(100);
                }

                Debug.WriteLine($"FIREBASE TOKEN REC: {p.Token}");
                IAuthTokenProvider authTokenProvider = DependencyService.Resolve<IAuthTokenProvider>();
                if (!string.IsNullOrEmpty(authTokenProvider.Token))
                {
                    await SendDeviceInfo();
                }
            };

            Debug.WriteLine($"FIREBASE TOKEN: {CrossFirebasePushNotification.Current.Token}");

            CrossFirebasePushNotification.Current.OnNotificationReceived += async (s, p) =>
            {
                Debug.WriteLine("OnNotificationReceived");
                while (!CommonGlobalVariables.FinishedSetup)
                {
                    await Task.Delay(100);
                }

                if (!string.IsNullOrEmpty(DependencyService.Resolve<IAuthTokenProvider>().Token))
                {
                    await HandleNotificationReceived(p.Data);
                }
            };

            CrossFirebasePushNotification.Current.OnNotificationOpened += async (s, p) =>
            {
                Debug.WriteLine("OnNotificationOpened");
                while (!CommonGlobalVariables.FinishedSetup)
                {
                    await Task.Delay(100);
                }

                if (!string.IsNullOrEmpty(DependencyService.Resolve<IAuthTokenProvider>().Token))
                {
                    await HandleNotificationOpened(p.Data);
                }
            };

            CrossFirebasePushNotification.Current.OnNotificationError += (s, p) =>
            {
               Debug.WriteLine("OnNotificationError");
            };


            IsAppInTray = false;
        }

        private Task HandleNotificationReceived(IDictionary<string, object> notificationData)
        {
            if (!IsAppInTray && Device.RuntimePlatform != Device.iOS)
            {
                INotificationManager notificationManager = DependencyService.Resolve<INotificationManager>();

                return notificationManager.Show(new NotificationRequest
                {
                    Body = notificationData.GetValueIfKeyContains("body"),
                    Title = notificationData.GetValueIfKeyContains("title"),
                    Data = notificationData,
                });
            }
            return Task.CompletedTask;
        }

        private async Task HandleNotificationOpened(IDictionary<string, object> notificationData)
        {
            if (notificationData != null &&
                notificationData.TryGetIntValue(Notifications.NEWS_ID, out int NewsId))
            {
                INewsTransaction newsTransaction = DependencyService.Resolve<INewsTransaction>();
                NewsDetailsDto newsDetails = await newsTransaction.GetNewsDetail(NewsId);
                if (newsDetails != null)
                {
                    IServerUrl serverUrl = DependencyService.Resolve<IServerUrl>();
                    string imageUrl = serverUrl.BuildUrl("News/GetNewsMainPhoto", extension: "Services") + "?Id=";
                    NewsDetailsViewModel newsViewModel = new NewsDetailsViewModel
                    {
                        Title = newsDetails.Title,
                        PublishStart = newsDetails.PublishStart,
                        MainPhotoUrl = newsDetails.HasImage ? imageUrl + newsDetails.Id : null,
                        Content = newsDetails.Content,
                    };
                    await MainNavigator.Current.GoToPageAsync(new NewsDetailsPage(newsViewModel));
                }
            }
        }

        private async Task SendDeviceInfo()
        {
            IApplicationInstance applicationInstance = DependencyService.Resolve<IApplicationInstance>();
            IUserTransaction userTransaction = DependencyService.Resolve<IUserTransaction>();
            await userTransaction.SendUserDeviceInfo(new PublicMobileDeviceDto
            {
                AppVersion = VersionTracking.CurrentVersion,
                DeviceModel = DeviceInfo.Model,
                DeviceType = DeviceInfo.Platform.ToString(),
                FirebaseTokenKey = CrossFirebasePushNotification.Current.Token,
                LastLoginDate = DateTime.Now,
                Imei = applicationInstance.Id,
                Osversion = DeviceInfo.Version.ToString(),
            }).ConfigureAwait(false);
        }

        private void OnTranslatorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetStyles();
        }

        private void SetStyles()
        {
            PickerStyle.Setters.Add(new Setter
            {
                Property = TLPicker.DialogCancelButtonTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Cancel"]
            });
            tlNativePicker.Setters.Add(new Setter
            {
                Property = TLNativePicker.DialogCancelButtonTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Cancel"]
            });

            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogTitleProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DateTimePickerTitle"]
            });
            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogCancelTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Cancel"]
            });
            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogOkayTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Select"]
            });
            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogYearTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DateTimeYear"]
            });
            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogMonthTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DateTimeMonth"]
            });
            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogDayTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DateTimeDay"]
            });
            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogHoursTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DateTimeHour"]
            });
            DateTimePickerStyle.Setters.Add(new Setter
            {
                Property = TLDateTimePicker.DialogMinutesTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DateTimeMinute"]
            });
            MultiPickerStyle.Setters.Add(new Setter
            {
                Property = TLMultiPicker.DialogCancelButtonTextProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Cancel"]
            });
            tlResponsiveTable.Setters.Add(new Setter
            {
                Property = TLResponsiveTable.TotalLabelProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Total"]
            });

            EntryForEgnLnch.Setters.Add(new Setter
            {
                Property = TLEntryWithType.EGNLabelProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/EGN"]
            });
            EntryForEgnLnch.Setters.Add(new Setter
            {
                Property = TLEntryWithType.LNCHLabelProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/LNCH"]
            });
            EntryForEgnLnch.Setters.Add(new Setter
            {
                Property = TLEntryWithType.FORIDLabelProperty,
                Value = TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/FORID"]
            });
        }
    }
}
