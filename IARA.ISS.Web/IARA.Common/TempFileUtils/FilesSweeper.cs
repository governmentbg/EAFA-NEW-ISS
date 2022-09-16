using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Timers;
using IARA.Logging.Abstractions.Interfaces;

namespace IARA.Common.TempFileUtils
{
    public class FilesSweeper : IFilesSweeper
    {
        private readonly object padlock;
        private IExtendedLogger logger;
        private ConcurrentQueue<string> queue;
        private System.Timers.Timer timer;
        public FilesSweeper(IExtendedLogger logger)
        {
            this.logger = logger;
            padlock = new object();
            queue = new ConcurrentQueue<string>();
            timer = new System.Timers.Timer();
            this.timer.Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;
            this.timer.Elapsed += this.Timer_Elapsed;
        }

        public void AddFileForRemoval(TempFileStream fileStream)
        {
            fileStream.FileRealeased += this.FileStream_FileRealeased;
            queue.Enqueue(fileStream.FileFullName);
            Start();
        }

        public void Start()
        {
            if (!this.timer.Enabled)
            {
                this.timer.Start();
            }
        }

        private void FileStream_FileRealeased(object sender, FileReleasedEventArgs e)
        {
            File.Delete(e.FileFullName);
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (Monitor.TryEnter(padlock))
            {
                try
                {
                    timer.Stop();

                    while (queue.Count > 0 && queue.TryPeek(out string fileFullPath))
                    {
                        try
                        {
                            if (File.Exists(fileFullPath))
                            {
                                try
                                {
                                    File.Delete(fileFullPath);
                                }
                                catch (UnauthorizedAccessException)
                                {
                                    queue.TryDequeue(out fileFullPath);
                                    queue.Enqueue(fileFullPath);
                                    Thread.Sleep(500);
                                    continue;
                                }
                            }

                            queue.TryDequeue(out fileFullPath);
                        }
                        catch (Exception ex)
                        {
                            logger.LogException(ex);
                        }
                    }
                }
                finally
                {
                    Monitor.Exit(padlock);
                    timer.Start();
                }
            }
        }
    }
}
