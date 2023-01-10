using System;
using System.Threading.Tasks;
using IARA.Mobile.Application;
using IARA.Mobile.Application.Extensions;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Shared.Extensions;
using IARA.Mobile.Shared.Popups;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Utilities
{
    public class ConnectivityUtility : IConnectivity
    {
        private bool healthChecherRunning;

        public ConnectivityUtility()
        {
            Connectivity.ConnectivityChanged += ConnectivityConnectivityChanged;
        }

        public event EventHandler<InternetStatus> ConnectivityChanged;
        public event EventHandler OfflineDataPosted;

        public void RunServerHealthChecker()
        {
            if (healthChecherRunning)
            {
                return;
            }

            healthChecherRunning = true;
            CommonGlobalVariables.InternetStatus = InternetStatus.Disconnected;
            ConnectivityChanged?.Invoke(this, InternetStatus.Disconnected);
            IRestClient restfulClient = DependencyService.Resolve<IRestClient>();

            Device.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                Task.Run(async () =>
                {
                    if (CommonGlobalVariables.InternetStatus == InternetStatus.Connected)
                    {
                        return;
                    }

                    if (await restfulClient.HealthCheckAsync().IsSuccessfulResult())
                    {
                        CommonGlobalVariables.InternetStatus = InternetStatus.Connected;
                        // Events have to execute on the main thread
                        await Device.InvokeOnMainThreadAsync(
                            () => ConnectivityChanged?.Invoke(this, InternetStatus.Connected)
                        );
                        await CheckForOutdatedVersion();
                        await PostOfflineData();
                        healthChecherRunning = false;
                        await Device.InvokeOnMainThreadAsync(
                            () => OfflineDataPosted?.Invoke(this, EventArgs.Empty)
                        );
                    }
                });
                return CommonGlobalVariables.InternetStatus != InternetStatus.Connected;
            });
        }

        private void ConnectivityConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (e.NetworkAccess == NetworkAccess.None || e.NetworkAccess == NetworkAccess.Unknown)
            {
                RunServerHealthChecker();
            }
        }

        private async Task CheckForOutdatedVersion()
        {
            IStartupTransaction startup = DependencyService.Resolve<IStartupTransaction>();
            bool isAppOutdated = await startup.IsAppOutdated(VersionExtensions.GetBuilderNumber(), Device.RuntimePlatform);

            if (isAppOutdated)
            {
                await PopupNavigation.Instance.PushAsync(new NewMajorVersionPopup());
            }
        }

        private async Task PostOfflineData()
        {
            IStartupTransaction startup = DependencyService.Resolve<IStartupTransaction>();
            await startup.PostOfflineData();

            IInspectionsTransaction inspections = DependencyService.Resolve<IInspectionsTransaction>();
            await inspections.PostOfflineInspections();
        }
    }
}
