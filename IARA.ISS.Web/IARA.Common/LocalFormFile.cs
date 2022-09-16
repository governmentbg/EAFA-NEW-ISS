using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IARA.Common
{
    public class LocalFormFile : IFormFile
    {
        private MemoryStream fileStream;

        public LocalFormFile(byte[] fileArray, string fileName, string contentType)
        {
            this.fileStream = new MemoryStream(fileArray);
            this.FileName = fileName;
            this.ContentType = contentType;
        }

        public string ContentType { get; private set; }

        public string ContentDisposition => throw new NotImplementedException();

        public IHeaderDictionary Headers => throw new NotImplementedException();
        public string Name => throw new NotImplementedException();

        public long Length { get { return fileStream.Length; } }

        public string FileName { get; private set; }

        public void CopyTo(Stream target)
        {
            fileStream.Position = 0;
            fileStream.CopyTo(target);
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            fileStream.Position = 0;
            return fileStream.CopyToAsync(target, cancellationToken);
        }

        public Stream OpenReadStream()
        {
            fileStream.Position = 0;
            return fileStream;
        }
    }
}
