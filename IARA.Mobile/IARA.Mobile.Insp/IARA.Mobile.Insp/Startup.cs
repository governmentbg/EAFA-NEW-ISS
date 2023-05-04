using System;
using System.IO;
using System.Threading.Tasks;
using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Infrastructure;
using IARA.Mobile.Insp.Application;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.FlyoutPages.InspectionsPage;
using IARA.Mobile.Insp.FlyoutPages.LoadingIndicatorPage;
using IARA.Mobile.Insp.FlyoutPages.LoginPage;
using IARA.Mobile.Insp.Infrastructure;
using IARA.Mobile.Insp.Menu;
using IARA.Mobile.Shared.Extensions;
using IARA.Mobile.Shared.Helpers;
using IARA.Mobile.Shared.Menu;
using IARA.Mobile.Shared.Popups;
using IARA.Mobile.Shared.ResourceTranslator;
using IARA.Mobile.Shared.Views;
using Microsoft.Extensions.DependencyInjection;
using Rg.Plugins.Popup.Services;
using TechnoLogica.Xamarin.Core;
using TechnoLogica.Xamarin.ResourceTranslator;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Insp
{
    public class Startup : IStartup
    {
        private const string StartupData = nameof(StartupData);

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMain()
                .AddInfrastructure()
                .AddCommonInfrastructure()
                .AddApplication()
                .AddShared();

            DependencyService.Resolve<IPlatformSpecificServices>()
                .AddPlatformSpecificServices(services);
        }

        public void Configure(IApplicationBuilder builder, bool isInitialCall, string state)
        {
            // Setup variables inside IStates to make sure if one step throws an exception, the variables are accessable by other steps
            builder.Call<IStates>(SetupVariables);

            if (isInitialCall)
            {
                // Initializes global variables and which server the app will use and font size
                builder.Call<IServerUrl, ISettings>(Init);
            }

            // Health check
            builder.Call<IStartupTransaction>(HealthCheck);

            // Checks if app is outdated
            builder.EndWhen<IStartupTransaction, ICommonLogout>(CheckAppOutdated);

            if (state != CommonConstants.NewLogin)
            {
                // Checks if user is logged in
                builder.EndWhen<IAuthTokenProvider>(UserLoggedIn);

                // Checks if app had it's major version changed
                builder.EndWhen<ISettings, ICommonLogout>(CheckMajourVersionChanged);

                // Checks if user has to change password
                builder.EndWhen<ICurrentUser>(UserChangePassword);
            }

            // Calls the migration script
            builder.Call<IAppDbMigration>(MigrateDatabase);

            if (state == CommonConstants.NewLogin)
            {
                // Pull startup data while showing a progress bar
                builder.Call<IStates, IStartupTransaction>(PullStartupDataProgress, onMainThread: true);

                // Logout if startup data couldn't be pulled
                builder.EndWhen<IStates, ICommonLogout, ISettings>(LogoutOnStartupFailFromLogin);
            }
            else
            {
                // Pull startup data in the background
                builder.Call<IStates, IStartupTransaction>(PullStartupData);

                // Logout if startup data couldn't be pulled
                builder.EndWhen<IStates, ICommonLogout, ISettings>(LogoutOnStartupFail);
            }

            // Send device info
            builder.Call<IStartupTransaction, IApplicationInstance>(SendDeviceInfo);

            // Load initial resources
            builder.Call(LoadInitialResources);

            // Check if device is allowed
            builder.Call<IStartupTransaction, IApplicationInstance>(CheckIfDeviceAllowed);

            if (state != CommonConstants.NewLogin)
            {
                // Post offline stored data
                builder.Call<IStartupTransaction, IInspectionsTransaction>(PostOfflineData);
            }

            // Set main page
            builder.Call(NavigateToMainPage, onMainThread: true);

            // Set login as successful
            builder.Call<ISettings>(SetLoginAsSuccessful);
        }

        private void SetupVariables(IStates states)
        {
            states.Set(StartupData, false);
        }

        private void Init(IServerUrl serverUrl, ISettings settings)
        {
            CommonGlobalVariables.PullItemsCount = Device.Idiom == TargetIdiom.Phone ? 20 : 40;
            CommonGlobalVariables.DatabasePath = Path.Combine(FileSystem.AppDataDirectory, "IARA.db3");

            Constants.InspectionFileTypeCode = Device.RuntimePlatform == Device.Android
                ? "MobileInspections"
                : "Inspections";
#if DEBUG
            serverUrl.Environment = Environments.DEVELOPMENT_LOCAL;
#elif PRODRELEASE
            serverUrl.Environment =  Environments.PRODUCTION;
#else
            serverUrl.Environment = Environments.STAGING;
#endif

            App.Current.SetFontSize(settings.FontSize);
        }

        private Task HealthCheck(IStartupTransaction startUp)
        {
            return startUp.HealthCheck();
        }

        private async Task<bool> CheckAppOutdated(IStartupTransaction startUp, ICommonLogout logout)
        {
            if (CommonGlobalVariables.InternetStatus == InternetStatus.Connected)
            {
                bool isAppOutdated = await startUp.IsAppOutdated(VersionExtensions.GetBuilderNumber(), Device.RuntimePlatform);

                if (isAppOutdated)
                {
                    logout.DeleteLocalInfo(false);
                    await PopupNavigation.Instance.PushAsync(new NewMajorVersionPopup());
                    return false;
                }
            }

            return true;
        }

        private bool UserLoggedIn(IAuthTokenProvider authTokenProvider)
        {
            if (string.IsNullOrEmpty(authTokenProvider.Token))
            {
                Translator.Current.LoadOfflineResources();
                App.Current.SetMainPage(new LoginPage());

                return false;
            }

            return true;
        }

        private bool CheckMajourVersionChanged(ISettings settings, ICommonLogout logout)
        {
            string currentVersion = VersionTracking.CurrentVersion;
            string lastVersion = settings.LastVersion;

            if (currentVersion == "DEBUG" || lastVersion == "DEBUG")
            {
                return true;
            }

            int lastMajorVersion = int.Parse(lastVersion.Split('.')[0]);
            int currentMajorVersion = int.Parse(currentVersion.Split('.')[0]);

            if (lastMajorVersion < currentMajorVersion)
            {
                settings.LastVersion = currentMajorVersion.ToString();
                logout.DeleteLocalInfo();
                return false;
            }

            settings.LastVersion = currentMajorVersion.ToString();
            return true;
        }

        private bool UserChangePassword(ICurrentUser currentUser)
        {
            if (currentUser.MustChangePassword)
            {
                Translator.Current.LoadOfflineResources();
                App.Current.SetMainPage(new ChangePasswordPage());

                return false;
            }

            return true;
        }

        private void MigrateDatabase(IAppDbMigration migration)
        {
            migration.CheckForMigrations();
        }

        private async Task PullStartupDataProgress(IStates states, IStartupTransaction startUp)
        {
            LoadingIndicatorPage indicatorPage = new LoadingIndicatorPage();
            App.Current.SetMainPage(indicatorPage);
            int current = -1;
            double max = 1;

            void CountCallback(int count)
            {
                max = count;
            }

            void FinishCallback()
            {
                indicatorPage.ProgressTo(1 / (max - current));
                current++;
            }

            bool result = await startUp.GetInitialData(true, CountCallback, FinishCallback);
            states.Set(StartupData, result);
        }

        private async Task PullStartupData(IStates states, IStartupTransaction startUp)
        {
            bool result = await startUp.GetInitialData(false, null, null);
            states.Set(StartupData, result);
        }

        private bool LogoutOnStartupFail(IStates states, ICommonLogout logout, ISettings settings)
        {
            if (!settings.SuccessfulLogin && !states.Get<bool>(StartupData))
            {
                logout.DeleteLocalInfo();
                return false;
            }

            return true;
        }

        private bool LogoutOnStartupFailFromLogin(IStates states, ICommonLogout logout, ISettings settings)
        {
            if (!settings.SuccessfulLogin && !states.Get<bool>(StartupData))
            {
                logout.DeleteLocalInfo(false);
                return false;
            }

            return true;
        }

        private Task SendDeviceInfo(IStartupTransaction startUp, IApplicationInstance applicationInstance)
        {
            return startUp.SendUserDeviceInfo(new PublicMobileDeviceDto
            {
                AppVersion = VersionTracking.CurrentVersion,
                DeviceModel = DeviceInfo.Model,
                DeviceType = DeviceInfo.Platform.ToString(),
                //FirebaseTokenKey = CrossFirebasePushNotification.Current.Token,
                LastLoginDate = DateTime.Now,
                Imei = applicationInstance.Id,
                Osversion = DeviceInfo.Version.ToString(),
            });
        }

        private void LoadInitialResources()
        {
            App.Current.LoadInitialResources();
        }

        private async Task CheckIfDeviceAllowed(IStartupTransaction startUp, IApplicationInstance appInstance)
        {
            string imei = appInstance.Id;

            while (!await startUp.IsDeviceAllowed(imei))
            {
                const string group = nameof(GroupResourceEnum.Common);

                await App.Current.MainPage.DisplayAlert(
                    TranslateExtension.Translator[group + "/DeviceNotAllowedTitle"],
                    string.Format(TranslateExtension.Translator[group + "/DeviceNotAllowedMessage"], imei),
                    TranslateExtension.Translator[group + "/Okay"]
                );
            }
        }

        private async Task PostOfflineData(IStartupTransaction startUp, IInspectionsTransaction inspections)
        {
            await startUp.PostOfflineData();
            await inspections.PostOfflineInspections();
        }

        private async Task NavigateToMainPage()
        {
            App.Current.SetMainPage(new MainNavigator(new MainFlyoutPage()));
            await MainNavigator.Current.GoToPageAsync(nameof(InspectionsPage));
        }

        private void SetLoginAsSuccessful(ISettings settings)
        {
            settings.SuccessfulLogin = true;
        }
    }
}
