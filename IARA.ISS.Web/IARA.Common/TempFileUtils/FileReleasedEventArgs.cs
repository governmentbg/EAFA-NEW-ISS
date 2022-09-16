using System;

namespace IARA.Common.TempFileUtils
{
    public class FileReleasedEventArgs : EventArgs
    {
        public readonly string FileFullName;
        public FileReleasedEventArgs(string path)
        {
            FileFullName = path;
        }
    }
}
