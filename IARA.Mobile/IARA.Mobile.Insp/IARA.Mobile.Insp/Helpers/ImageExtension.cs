using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.Helpers
{
    [ContentProperty(nameof(Path))]
    [AcceptEmptyServiceProvider]
    public class ImageExtension : IMarkupExtension<FileImageSource>
    {
        public const string DefaultExtension = "png";

        public string Path { get; set; }
        public string Extension { get; set; } = DefaultExtension;

        public FileImageSource ProvideValue(IServiceProvider serviceProvider)
        {
            return Convert(Path, Extension);
        }

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider)
        {
            return ProvideValue(serviceProvider);
        }

        public static FileImageSource Convert(string path, string extension = DefaultExtension)
        {
            if (path == null)
            {
                throw new InvalidOperationException($"Cannot convert null to {typeof(ImageSource)}");
            }

            if (Device.RuntimePlatform == Device.UWP)
            {
                path = System.IO.Path.Combine($"Images/{path}.{extension}");
            }

            return (FileImageSource)ImageSource.FromFile(path);
        }
    }
}
