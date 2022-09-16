using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamTest.Models;

namespace XamTest.Helpers
{
    /// <summary>
    /// This class allows you to pick a media file and store it inside of the cached directory.
    /// </summary>
    public static class TLMediaPicker
    {
        /// <summary>
        /// This will open the user's file storage to search for a photo. <para />
        /// NOTE: After the user picks an image it will get saved to the this app's cache directory
        /// and the method will return the <see cref="TLFileResult"/> with the new path.
        /// </summary>
        /// <param name="options">There is no documentation from Xamarin about this class.</param>
        /// <returns>The <see cref="TLFileResult"/> with the location from the cache directory or null if a file wasn't picked.</returns>
        public static Task<TLFileResult?> PickPhotoAsync(MediaPickerOptions? options = null)
        {
            return PickAsync(MediaPicker.PickPhotoAsync(options), false);
        }

        /// <summary>
        /// This will open the user's camera to capture a photo. <para />
        /// NOTE: After the user captures an image it will get saved to the this app's cache directory
        /// and the method will return the <see cref="TLFileResult"/> with the new path.
        /// </summary>
        /// <param name="options">There is no documentation from Xamarin about this class.</param>
        /// <returns>The <see cref="TLFileResult"/> with the location from the cache directory or null if a file wasn't picked.</returns>
        public static Task<TLFileResult?> CapturePhotoAsync(MediaPickerOptions? options = null)
        {
            return PickAsync(MediaPicker.CapturePhotoAsync(options), true);
        }

        /// <summary>
        /// This will open the user's file storage to search for a video. <para />
        /// NOTE: After the user picks an video it will get saved to the this app's cache directory
        /// and the method will return the <see cref="TLFileResult"/> with the new path.
        /// </summary>
        /// <param name="options">There is no documentation from Xamarin about this class.</param>
        /// <returns>The <see cref="TLFileResult"/> with the location from the cache directory or null if a file wasn't picked.</returns>
        public static Task<TLFileResult?> PickVideoAsync(MediaPickerOptions? options = null)
        {
            return PickAsync(MediaPicker.PickVideoAsync(options), false);
        }

        /// <summary>
        /// This will open the user's camera to record a video. <para />
        /// NOTE: After the user records a video it will get saved to the this app's cache directory
        /// and the method will return the <see cref="TLFileResult"/> with the new path.
        /// </summary>
        /// <param name="options">There is no documentation from Xamarin about this class.</param>
        /// <returns>The <see cref="TLFileResult"/> with the location from the cache directory or null if a file wasn't picked.</returns>
        public static Task<TLFileResult?> CaptureVideoAsync(MediaPickerOptions? options = null)
        {
            return PickAsync(MediaPicker.CaptureVideoAsync(options), false);
        }

        private static async Task<TLFileResult?> PickAsync(Task<FileResult?> pick, bool isCapturePhoto)
        {
            try
            {
                FileResult? photo = await pick;
                return await (
                    isCapturePhoto && Device.RuntimePlatform == Device.iOS
                        ? LoadiOSCapturedPhotoAsync(photo)
                        : LoadPickedMediaAsync(photo)
                );
            }
            catch
            {
                // The user canceled or something went wrong
                return null;
            }
        }

        private static async Task<TLFileResult?> LoadiOSCapturedPhotoAsync(FileResult? photo)
        {
            // Canceled
            if (photo == null)
            {
                return null;
            }

            // Use FFImageLoading to make it a reasonable size and other transformations
            TaskParameter ffImage = ImageService.Instance.LoadStream(async _ => await photo.OpenReadAsync());

            // iOS doesn't rotate the image to match the device orientation, so we have to rotate it 90 degrees here
            ffImage.Transform(new RotateTransformation(90));

            // Save the file into this app's cached directory
            string newFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            using Stream stream = await ffImage.AsPNGStreamAsync();
            using FileStream newStream = File.OpenWrite(newFilePath);

            await stream.CopyToAsync(newStream);

            return new TLFileResult(newFilePath, photo.FileName, photo.ContentType, stream.Length);
        }

        private static async Task<TLFileResult?> LoadPickedMediaAsync(FileResult? media)
        {
            // Canceled
            if (media == null)
            {
                return null;
            }

            // Save the file into this app's cached directory
            string newFilePath = Path.Combine(FileSystem.CacheDirectory, media.FileName);
            using Stream stream = await media.OpenReadAsync();
            using FileStream newStream = File.OpenWrite(newFilePath);

            await stream.CopyToAsync(newStream);

            return new TLFileResult(newFilePath, media.FileName, media.ContentType, stream.Length);
        }
    }
}
