using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using IARA.Mobile.Application.Extensions;
using IARA.Mobile.Application.Interfaces.Utilities;
using MimeTypes;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Xamarin.Essentials;

namespace IARA.Mobile.Insp.UWP.Extensions
{
    public class WindowsDownloader : IDownloader
    {
        private readonly IServerUrl _serverUrl;
        private readonly IAuthTokenProvider _authTokenProvider;
        private readonly IExceptionHandler _exceptionHandler;

        public WindowsDownloader(IServerUrl serverUrl, IAuthTokenProvider authTokenProvider, IExceptionHandler exceptionHandler)
        {
            _serverUrl = serverUrl ?? throw new ArgumentNullException(nameof(serverUrl));
            _authTokenProvider = authTokenProvider ?? throw new ArgumentNullException(nameof(authTokenProvider));
            _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
        }

        public async Task<bool> DownloadFile(string fileName, string contentType, string url, object parameters)
        {
            PermissionStatus status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                return false;
            }

            StorageFile file = null;

            try
            {
                Uri source = _serverUrl.BuildUri(url, parameters, "Services");

                string extension = Path.GetExtension(fileName);

                if (string.IsNullOrEmpty(extension))
                {
                    try
                    {
                        extension = MimeTypeMap.GetExtension(contentType);
                    }
                    catch (Exception ex)
                    {
                        await _exceptionHandler.HandleException(ex);
                        extension = string.Empty;
                    }
                }

                FileSavePicker savePicker = new FileSavePicker
                {
                    SuggestedStartLocation = PickerLocationId.Downloads,
                    SuggestedFileName = fileName
                };
                savePicker.FileTypeChoices.Add(GetNameFromMimeType(contentType), new List<string>() { extension });

                file = await savePicker.PickSaveFileAsync();

                if (file != null)
                {
                    BackgroundDownloader downloader = new BackgroundDownloader();
                    downloader.SetRequestHeader("Authorization", "Bearer " + _authTokenProvider.Token);
                    DownloadOperation download = downloader.CreateDownload(source, file);

                    await download.StartAsync();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                if (file != null)
                {
                    await file.DeleteAsync();
                }

                await _exceptionHandler.HandleException(ex);

                return false;
            }
        }

        private string GetNameFromMimeType(string mimeType)
        {
            string[] split = mimeType.Split('/');
            return split[0].CapitalizeFirstLetter();
        }
    }
}
