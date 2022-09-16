using System;
using System.IO;
using System.Threading;

namespace IARA.Common.TempFileUtils
{
    public static class TempFileBuilder
    {
        private static object directoryPadlock = new object();
        private const string TEMP_DIR = "TEMP";

        private static long fileCounter = 0;

        public static FileStream BuildTempFile(string fileName)
        {
            var tempDir = new DirectoryInfo(Path.Combine(BaseDirectory, TEMP_DIR));
            if (!tempDir.Exists)
            {
                lock (directoryPadlock)
                {
                    tempDir.Create();
                }
            }

            return new FileStream(Path.Combine(tempDir.FullName, fileName), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        }

        public static string GenerateUniqueFileName()
        {
            unchecked
            {
                long count = Interlocked.Increment(ref fileCounter);
                return $"{DateTime.Now.ToString("HHmmssfff")}{count}";
            }
        }


        private static string baseDirectory;
        public static string BaseDirectory
        {
            get
            {
                if (baseDirectory == null)
                {
                    baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                }

                return baseDirectory;
            }
        }

    }
}
