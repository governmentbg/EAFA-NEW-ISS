using CoreGraphics;
using Foundation;
using System;
using System.Diagnostics;
using System.IO;
using UIKit;
using Xamarin.Forms;
using XamTest.Interfaces;
using XamTest.iOS;

[assembly: Dependency(typeof(ImageCompression))]
namespace XamTest.iOS
{
    public class ImageCompression : IImageCompressionService
    {
        public byte[] ResizeImage(byte[] imageData, string destinationPath, float maxWidth, float maxHeight, int compressionPercentage = 100)
        {
            UIImage originalImage = ImageFromByteArray(imageData);
            if (originalImage != null)
            {
                UIImage resizedImage = ResizeUIImage(originalImage, maxWidth, maxHeight);
                if (resizedImage != null)
                {
                    Debug.WriteLine($"Original:Width{originalImage.Size.Width}, Height:{originalImage.Size.Height} Resized:Width:{resizedImage.Size.Width}, Height: {resizedImage.Size.Height}");
                    nfloat compressionQuality = (nfloat)(compressionPercentage / 100.0);
                    var compressedImage = resizedImage.AsJPEG(compressionQuality).ToArray();
                    var stream = new FileStream(destinationPath, FileMode.Create);
                    stream.Write(compressedImage, 0, compressedImage.Length);
                    stream.Flush();
                    stream.Close();
                    return compressedImage;
                }
            }
            return null;
        }

        public UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null) return null;

            UIImage image;
            try
            {
                image = new UIImage(NSData.FromArray(data));

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exeption:{ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }

            return image;
        }

        public UIImage ResizeUIImage(UIImage sourceImage, float maxWidth, float maxHeight)
        {
            var sourceSize = sourceImage.Size;
            var maxResizeFactor = Math.Min(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
            if (maxResizeFactor > 1) return sourceImage;
            var width = maxResizeFactor * sourceSize.Width;
            var height = maxResizeFactor * sourceSize.Height;
            UIGraphics.BeginImageContext(new CGSize(width, height));
            sourceImage.Draw(new CGRect(0, 0, width, height));
            var resultImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            return resultImage;
        }
    }
}