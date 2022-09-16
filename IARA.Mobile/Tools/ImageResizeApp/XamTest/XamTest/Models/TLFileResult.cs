namespace XamTest.Models
{
    public class TLFileResult
    {
        /// <summary>
        /// Gets the full path and filename.
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// The name of the file.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file's content type as a MIME type (eg: `image/png`).
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// The size of the file
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Initializes the <see cref="TLFileResult"/>
        /// </summary>
        public TLFileResult(string fullPath, string fileName, string contentType, long fileSize)
        {
            FullPath = fullPath;
            FileName = fileName;
            ContentType = contentType;
            FileSize = fileSize;
        }
    }
}
