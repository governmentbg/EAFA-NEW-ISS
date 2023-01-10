using System;
using System.IO;
using System.Threading.Tasks;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Domain.Models;
using Xamarin.Essentials;

namespace IARA.Mobile.Pub.iOS.Extensions
{
    public class IosDownloader : IDownloader
    {
        private readonly IRestClient _restClient;
        public IosDownloader(IRestClient restClient)
        {
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        }

        public async Task<bool> DownloadFile(string fileName, string contentType, string url, object parameters = null)
        {
            PermissionStatus status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                return false;
            }

            HttpResult<FileResponse> result = await _restClient.GetAsync<FileResponse>(url, parameters);

            if (!result.IsSuccessful && result.Content == null)
            {
                return false;
            }
            string path = Path.Combine(FileSystem.CacheDirectory, fileName);
            File.WriteAllBytes(path, result.Content.File);
            await Share.RequestAsync(new ShareFileRequest
            {
                Title = fileName,
                File = new ShareFile(path)
            });

            return true;
        }
    }
}
