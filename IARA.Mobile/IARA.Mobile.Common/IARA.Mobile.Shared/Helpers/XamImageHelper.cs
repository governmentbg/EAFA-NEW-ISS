using System;
using System.IO;
using Plugin.TechnoLogica.XamImage;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Shared.Helpers
{
    public static class XamImageHelper
    {
        private const string JpegMimeType = "image/jpeg";
        private const string JpegExtension = ".jpeg";

        public static TLFileResult ImageResize(this TLFileResult file, float maxWidth, float maxHeight, int compressionPercentage = 100)
        {
            if (CrossXamImage.IsSupported &&
                File.Exists(file.FullPath) &&
                file.ContentType.StartsWith("image/"))
            {
                string fileName = Guid.NewGuid().ToString("N") + JpegExtension;
                string destinationPath = Path.Combine(Path.GetDirectoryName(file.FullPath), fileName);
                byte[] originalImage = File.ReadAllBytes(file.FullPath);
                byte[] newImage = CrossXamImage.Current.ResizeImage(originalImage, destinationPath, maxWidth, maxHeight, compressionPercentage);

                if (newImage == null)
                {
                    return file;
                }

                if (File.Exists(file.FullPath))
                {
                    File.Delete(file.FullPath);
                }

                file = new TLFileResult(destinationPath, fileName, JpegMimeType, newImage.Length);
            }

            return file;
        }
    }
}
