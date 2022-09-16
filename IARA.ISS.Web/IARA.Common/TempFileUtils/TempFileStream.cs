using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using IARA.Common.TempFileUtils;

namespace IARA.Common
{
    public class TempFileStream : Stream
    {
        private FileStream fileStream;

        public TempFileStream(string fileName)
        {
            this.fileStream = TempFileBuilder.BuildTempFile(fileName);
        }

        private event EventHandler<FileReleasedEventArgs> fileReleased = delegate { };

        public event EventHandler<FileReleasedEventArgs> FileRealeased
        {
            add
            {
                fileReleased += value;
            }
            remove
            {
                fileReleased -= value;
            }
        }

        public string FileFullName { get => fileStream.Name; }

        public override bool CanRead => fileStream.CanRead;

        public override bool CanSeek => fileStream.CanSeek;

        public override bool CanTimeout => fileStream.CanTimeout;
        public override bool CanWrite => fileStream.CanWrite;

        public override long Length => fileStream.Length;

        public override long Position { get => fileStream.Position; set => fileStream.Position = value; }

        public override int ReadTimeout { get => fileStream.ReadTimeout; set => fileStream.ReadTimeout = value; }

        public override int WriteTimeout { get => fileStream.WriteTimeout; set => fileStream.WriteTimeout = value; }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return fileStream.BeginRead(buffer, offset, count, callback, state);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            return fileStream.BeginWrite(buffer, offset, count, callback, state);
        }

        public override void Close()
        {
            fileStream.Close();
            base.Close();
        }

        public override void CopyTo(Stream destination, int bufferSize)
        {
            fileStream.CopyTo(destination, bufferSize);
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            return fileStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override ValueTask DisposeAsync()
        {
            return fileStream.DisposeAsync();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            return fileStream.EndRead(asyncResult);
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            fileStream.EndWrite(asyncResult);
        }

        public override void Flush()
        {
            fileStream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return fileStream.FlushAsync(cancellationToken);
        }

        public override int GetHashCode()
        {
            return fileStream.Name.GetHashCode();
        }

        public override object InitializeLifetimeService()
        {
            return fileStream.InitializeLifetimeService();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return fileStream.Read(buffer, offset, count);
        }

        public override int Read(Span<byte> buffer)
        {
            return fileStream.Read(buffer);
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return fileStream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return fileStream.ReadAsync(buffer, cancellationToken);
        }

        public override int ReadByte()
        {
            return fileStream.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return fileStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            fileStream.SetLength(value);
        }

        public override string ToString()
        {
            return fileStream.Name;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            fileStream.Write(buffer, offset, count);
        }
        public override void Write(ReadOnlySpan<byte> buffer)
        {
            fileStream.Write(buffer);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            return fileStream.WriteAsync(buffer, offset, count, cancellationToken);
        }

        public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            return fileStream.WriteAsync(buffer, cancellationToken);
        }

        public override void WriteByte(byte value)
        {
            fileStream.WriteByte(value);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                fileStream.Dispose();
                base.Dispose(disposing);
                OnFileReleased();
            }
        }

        private void OnFileReleased()
        {
            foreach (var handler in fileReleased.GetInvocationList())
            {
                try
                {
                    handler.DynamicInvoke(new object[] { this, new FileReleasedEventArgs(this.fileStream.Name) });
                }
                catch (Exception)
                {

                }
            }
        }
    }
}
