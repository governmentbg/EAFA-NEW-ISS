using Xamarin.Forms;
using XamTest.ViewModels;

namespace XamTest.Models
{
    public class ImageItem : BaseViewModel
    {
        private ImageSource _imageSource;
        public ImageSource ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }
        public string Size { get; set; }
        public string FileName { get; set; }
        public string CompressionPercentage { get; set; }
    }
}
