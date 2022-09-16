using Android.Graphics;
using System;
using System.Diagnostics;
using System.IO;

namespace Plugin.TechnoLogica.XamImage
{
    public class XamImageImplementation : IXamImage
    {
        public byte[] ResizeImage(byte[] imageData, string destinationPath, float maxWidth, float maxHeight, int compressionRate = 100)
        {
            var resizedImage = ResizeAndCompressBitmap(imageData, compressionRate, maxWidth, maxHeight);
            var stream = new FileStream(destinationPath, FileMode.Create);
            stream.Write(resizedImage, 0, resizedImage.Length);
            stream.Flush();
            stream.Close();
            return resizedImage;
        }

        private byte[] ResizeAndCompressBitmap(byte[] imageData, int compressionRate, float maxWidth, float maxHeight)
        {
            Bitmap resizedImage = ResizeBitmap(imageData, maxWidth, maxHeight);
            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, compressionRate, ms);
                return ms.ToArray();
            }
        }

        private Bitmap ResizeBitmap(byte[] imageData, float maxWidth, float maxHeight)
        {
            // Load the bitmap
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
            var maxResizeFactor = Math.Min(maxWidth / originalImage.Width, maxHeight / originalImage.Height);
            if (maxResizeFactor > 1) return originalImage;
            var width = maxResizeFactor * originalImage.Width;
            var height = maxResizeFactor * originalImage.Height;
            Debug.WriteLine($"Original:Width{originalImage.Width}, Height:{originalImage.Height} Resized:Width:{(int)width}, Height: {(int)height}");
            return Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);
        }
    }
}
