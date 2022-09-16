namespace XamTest.Interfaces
{
    public interface IImageCompressionService
    {
        /// <summary>
        /// Use this method to resize and compress images and store them on the device. Resizing the image keeps the aspect ratio.
        /// If the image dimensions are lower than the ones we want to resize to, the image will be only compressed and returned in the same dimensions.
        /// </summary>
        /// <param name="imageData">Actual image in byte array</param>
        /// <param name="destinationPath">Full path with file name to be stored in.</param>
        /// <param name="maxWidth">Maximum width</param>
        /// <param name="maxHeight">Maximum height</param>
        /// <param name="compressionRate">Compressing value - 100 means the image will remain the same quality, e.g. "80" will reduce the image quality with 20%</param>
        /// <returns>Returns the resized image</returns>
        byte[] ResizeImage(byte[] imageData, string destinationPath, float maxWidth, float maxHeight, int compressionRate = 100);
    }
}
