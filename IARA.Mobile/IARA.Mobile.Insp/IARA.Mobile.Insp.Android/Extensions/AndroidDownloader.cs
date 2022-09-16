using Android.App;
using Android.Content;
using IARA.Mobile.Application.Interfaces.Utilities;
using System;
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
            PermissionStatus status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                return false;
            }

            try
            {
                DownloadManager.Request request = new DownloadManager.Request(Android.Net.Uri.Parse(_serverUrl.BuildUrl(url, parameters, "Services")));
                request.AddRequestHeader("Authorization", "Bearer " + _authTokenProvider.Token);
                request.SetDestinationInExternalPublicDir(Android.OS.Environment.DirectoryDownloads, fileName);
                request.SetNotificationVisibility(DownloadVisibility.VisibleNotifyCompleted);
                request.SetMimeType(contentType);

                DownloadManager manager = (DownloadManager)MainActivity.Current.GetSystemService(Context.DownloadService);
                manager.Enqueue(request);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
