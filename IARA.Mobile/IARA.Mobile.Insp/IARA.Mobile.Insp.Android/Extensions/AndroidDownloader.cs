using Android.App;
using IARA.Mobile.Application.Interfaces.Utilities;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace IARA.Mobile.Insp.Droid.Extensions
{
    public class AndroidDownloader : IDownloader
    {
        private readonly IServerUrl _serverUrl;
        private readonly IAuthTokenProvider _authTokenProvider;

        public AndroidDownloader(IServerUrl serverUrl, IAuthTokenProvider authTokenProvider)
        {
            _serverUrl = serverUrl ?? throw new ArgumentNullException(nameof(serverUrl));
            _authTokenProvider = authTokenProvider ?? throw new ArgumentNullException(nameof(authTokenProvider));
        }

        public async Task<bool> DownloadFile(string fileName, string contentType, string url, object parameters)
        {
            if (await Permissions.RequestAsync<Permissions.StorageWrite>() != PermissionStatus.Granted ||
                await Permissions.CheckStatusAsync<Permissions.StorageRead>() != PermissionStatus.Granted)
            {
                return false;
            }

            try
            {
                var manager = DownloadManager.FromContext(Android.App.Application.Context);
                var request = new DownloadManager.Request(Android.Net.Uri.Parse(_serverUrl.BuildUrl(url, parameters, "Services")));
                request.AddRequestHeader("Authorization", "Bearer " + _authTokenProvider.Token);
                request.SetNotificationVisibility(DownloadVisibility.VisibleNotifyCompleted);
                request.SetDestinationInExternalPublicDir(Android.OS.Environment.DirectoryDownloads, fileName);
                request.SetTitle(fileName);

                long downloadId = manager.Enqueue(request);
                Debug.WriteLine($"Download manager successfully started the download.");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
