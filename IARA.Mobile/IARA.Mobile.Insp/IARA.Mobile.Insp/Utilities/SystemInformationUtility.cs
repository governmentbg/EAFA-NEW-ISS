using IARA.Mobile.Application;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace IARA.Mobile.Insp.Utilities
{
    public class SystemInformationUtility : ISystemInformationProvider
    {
        private readonly IApplicationInstance applicationInstance;
        private readonly ICurrentUser currentUser;
        private readonly IStartupTransaction startupTransaction;
        private readonly IServerUrl serverUrl;

        public SystemInformationUtility(IApplicationInstance applicationInstance, ICurrentUser currentUser, IStartupTransaction startupTransaction, IServerUrl serverUrl)
        {
            this.applicationInstance = applicationInstance;
            this.currentUser = currentUser;
            this.startupTransaction = startupTransaction;
            this.serverUrl = serverUrl;
        }

        public async Task<List<string>> Get()
        {
            List<string> systemParamters = new List<string>
            {
                $"Application Id: {applicationInstance.Id}",
                $"OS: {DeviceInfo.Platform.ToString() + " " + DeviceInfo.VersionString}",
                $"Idiom: {DeviceInfo.Idiom}",
                $"Manufacturer: {DeviceInfo.Manufacturer}",
                $"Model: {DeviceInfo.Model}",
                $"Name: {DeviceInfo.Name}",
                $"AppVersion: {VersionTracking.CurrentVersion}",
                $"Location: {await CheckLocation()}",
                $"User: {currentUser.FirstName + " " + currentUser.LastName + " " + currentUser.Id}",
                $"Internet Status:{CommonGlobalVariables.InternetStatus}",
                $"Health Check: {await startupTransaction.HealthCheck()}",
                $"Environment: {serverUrl.Environment}",
            };

            return systemParamters;
        }

        private async Task<string> CheckLocation()
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                Location result = await Geolocation.GetLocationAsync(request, new CancellationTokenSource().Token);
                return "Works!";
            }
            catch (FeatureNotSupportedException)
            {
                return "Feature Not Supported";
            }
            catch (FeatureNotEnabledException)
            {
                // Handle not enabled on device exception
                return "Feature not enabled";
            }
            catch (PermissionException)
            {
                // Handle permission exception
                return "No permission";
            }
            catch (Exception)
            {
                // Unable to get location
                return "Unable to get";
            }
        }
    }
}
