using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using XamTest.Interfaces;
using XamTest.Models;

namespace XamTest.ViewModels
{
    public class ImageCompressionViewModel : BaseViewModel
    {
        public ImageCompressionViewModel()
        {
            Photos = new ObservableCollection<ImageItem>();
            Title = "Image Compression";
            PickPhotoCommand = new Command(async () => await OnPickPhotoAsync());
            CapturePhotoCommand = new Command(async () => await OnCapturePhotoAsync());
        }

        public ICommand PickPhotoCommand { get; }
        public ICommand CapturePhotoCommand { get; }

        private ObservableCollection<ImageItem> _photos;
        public ObservableCollection<ImageItem> Photos
        {
            get => _photos;
            set => SetProperty(ref _photos, value);
        }

        private async Task OnPickPhotoAsync()
        {
            try
            {
                //var photo = await MediaPicker.CapturePhotoAsync();
                var photo = await MediaPicker.PickPhotoAsync();
                await LoadPhotoAsync(photo);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature is not supported on the device
            }
            catch (PermissionException pEx)
            {
                // Permissions not granted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        private async Task OnCapturePhotoAsync()
        {
            try
            {
                //var photo = await MediaPicker.CapturePhotoAsync();
                var photo = await MediaPicker.CapturePhotoAsync();
                await LoadPhotoAsync(photo);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Feature is not supported on the device
            }
            catch (PermissionException pEx)
            {
                // Permissions not granted
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CapturePhotoAsync THREW: {ex.Message}");
            }
        }

        private async Task LoadPhotoAsync(FileResult photo)
        {
            // save the file into local storage
            string fileName = "Original_" + photo.FileName;
            var originalFile = Path.Combine(FileSystem.CacheDirectory, fileName);
            using (var stream = await photo.OpenReadAsync())
            using (var newStream = File.OpenWrite(originalFile))
                await stream.CopyToAsync(newStream);

            using (var stream = await photo.OpenReadAsync())
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                byte[] image = ms.ToArray();

                Photos.Add(new ImageItem() { ImageSource = originalFile, Size = image.Length.ToString(), CompressionPercentage = "Original", FileName = photo.FileName });

                CompressImage(image, 70, photo.FileName);
                CompressImage(image, 50, photo.FileName);
                CompressImage(image, 30, photo.FileName);
                CompressImage(image, 10, photo.FileName);
                CompressImage(image, 5, photo.FileName);
            }
        }

        private byte[] CompressImage(byte[] image, int compressionPercentage, string fileName)
        {
            fileName = compressionPercentage.ToString() + '_' + fileName;
            string filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
            byte[] newImage = DependencyService.Get<IImageCompressionService>().ResizeImage(image, filePath, 1920, 1920, compressionPercentage);

            Photos.Add(new ImageItem() { ImageSource = filePath, Size = newImage.Length.ToString(), CompressionPercentage = compressionPercentage.ToString(), FileName = fileName });
            return newImage;
        }
    }
}
