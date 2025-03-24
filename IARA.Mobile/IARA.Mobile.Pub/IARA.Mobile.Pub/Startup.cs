using IARA.Mobile.Application;
using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Infrastructure;
using IARA.Mobile.Pub.Application;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Infrastructure;
using IARA.Mobile.Pub.Views.FlyoutPages;
using IARA.Mobile.Pub.Views.Menu;
using IARA.Mobile.Shared.Extensions;
using IARA.Mobile.Shared.Helpers;
using IARA.Mobile.Shared.Menu;
using IARA.Mobile.Shared.Popups;
using IARA.Mobile.Shared.ResourceTranslator;
using IARA.Mobile.Shared.Views;
using Microsoft.Extensions.DependencyInjection;
using Plugin.FirebasePushNotification;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using TechnoLogica.Xamarin.Core;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Pub
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
                builder.Call<IServerUrl, ISettings>(Init, true);
            }

            // Health check
            builder.Call<IStartupTransaction>(HealthCheck, true);

            // Checks if app is outdated
            builder.EndWhen<IStartupTransaction, ICommonLogout>(CheckAppOutdated);

            if (state != CommonConstants.NewLogin)
            {
                // Check if user is unregistered eAuth user
                builder.Call<IAuthTokenProvider, IAuthenticationProvider>(CheckEAuthLogin);

                // Checks if user is logged in
                builder.EndWhen<IAuthTokenProvider, ICurrentUser, ISettings>(UserLoggedIn);

                // Checks if user has to change password
                builder.EndWhen<ICurrentUser>(UserChangePassword);
            }

            // Calls the migration script
            builder.Call<IAppDbMigration>(MigrateDatabase);

            if (state == CommonConstants.NewLogin)
            {
                // Pull startup data while showing a progress bar
                builder.Call<IStates, IStartupTransaction>(PullStartupDataProgress, onMainThread: true);
            }
            else
            {
                // Pull startup data in the background
                builder.Call<IStates, IStartupTransaction>(PullStartupData);
            }

            // Logout if startup data couldn't be pulled
            builder.EndWhen<IStates, ICommonLogout, ISettings>(LogoutOnStartupFail);

            // Send device info
            builder.Call<IUserTransaction, IApplicationInstance>(SendDeviceInfo);

            // Load initial resources
            builder.Call(LoadInitialResources);

            if (state != CommonConstants.NewLogin)
            {
                // Post offline stored data
                builder.Call<IStartupTransaction, ICatchRecordsTransaction>(PostOfflineData);
            }

            // Set main page
            builder.Call<INomenclatureTransaction>(NavigateToMainPage, onMainThread: true);

            // Set login as successful
            builder.Call<ISettings>(SetLoginAsSuccessful);
        }

        private void Init(IServerUrl serverUrl, ISettings settings)
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag(
                settings.CurrentResourceLanguage.ToString().ToLower()
            );
            CommonGlobalVariables.PullItemsCount = Device.Idiom == TargetIdiom.Phone ? 20 : 40;
            CommonGlobalVariables.DatabasePath = Path.Combine(FileSystem.AppDataDirectory, "IARA.db3");
#if DEBUG
            serverUrl.Environment = Environments.STAGING;
#elif PRODRELEASE
            serverUrl.Environment = Environments.PRODUCTION;
#else
            serverUrl.Environment = Environments.STAGING;
#endif

            CommonGlobalVariables.FinishedSetup = true;
        }

        private void SetupVariables(IStates states)
        {
            states.Set(StartupData, false);
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

        private async Task CheckEAuthLogin(IAuthTokenProvider authTokenProvider, IAuthenticationProvider authProvider)
        {
            if (authTokenProvider.IsUnregisteredEAuthUser)
            {
                // Зачистване на валиден eAuth token, при НЕзавършена eAuth регистрация
                await authProvider.SoftLogout();
            }
        }

        private bool UserLoggedIn(IAuthTokenProvider authTokenProvider, ICurrentUser currentUser, ISettings settings)
        {
            if (string.IsNullOrEmpty(authTokenProvider.Token) || currentUser.Id == default)
            {
                CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag(
                settings.CurrentResourceLanguage.ToString().ToLower());
                Translator.Current.LoadOfflineResources();
                App.Current.SetMainPage(new PublicHomePage());

                return false;
            }

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

        private Task SendDeviceInfo(IUserTransaction userTransaction, IApplicationInstance applicationInstance)
        {
            return userTransaction.SendUserDeviceInfo(new PublicMobileDeviceDto
            {
                AppVersion = VersionTracking.CurrentVersion,
                DeviceModel = DeviceInfo.Model,
                DeviceType = DeviceInfo.Platform.ToString(),
                FirebaseTokenKey = CrossFirebasePushNotification.Current.Token,
                LastLoginDate = DateTime.Now,
                Imei = applicationInstance.Id,
                Osversion = DeviceInfo.Version.ToString(),
            });
        }

        private void LoadInitialResources()
        {
            App.Current.LoadInitialResources();
        }

        private async Task PostOfflineData(IStartupTransaction startUp, ICatchRecordsTransaction catchRecords)
        {
            await startUp.PostOfflineData();
            await catchRecords.PostOfflineCatchRecords();
        }

        private async Task NavigateToMainPage(INomenclatureTransaction nomenclature)
        {
            List<string> permissions = nomenclature.GetPermissions();

            App.Current.SetMainPage(new MainNavigator(
                new MainFlyoutPage(
                    CheckForPermission(permissions, ServerPermissions.ScientificFishingRead),
                    CheckForPermission(permissions, ServerPermissions.TicketsPublicRead),
                    CheckForPermission(permissions, ServerPermissions.CatchRecordsRead),
                    CheckForPermission(permissions, ServerPermissions.ReportRead),
                    CheckForPermission(permissions, ServerPermissions.ReportViolationSend),
                    CheckForPermission(permissions, ServerPermissions.ProfileRead)
                )
            ));
            await MainNavigator.Current.GoToPageAsync(nameof(HomePage));
        }

        private void SetLoginAsSuccessful(ISettings settings)
        {
            settings.SuccessfulLogin = true;
        }

        private static bool CheckForPermission(List<string> permissions, string permission)
        {
            return permissions?.Exists(f => f == permission) == true;
        }
    }
}