using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Humanizer;
using IARA.MigrationScript.Models;
using Timer = System.Timers.Timer;

namespace IARA.MigrationScript
{
    public abstract class BaseMigrate : IDisposable
    {
        public static string OldDbCS { get; set; }
        public static string NewDbCS { get; set; }
        public static int ThreadCount { get; set; }
        public static int TimeToNextThread { get; set; }
        public static int ChunkSize { get; set; }

        private readonly int startChunk;
        private string currentRunPath;
        private int exceptionIndex;
        private bool hasFinished;

        protected const string ScriptPath = "Scripts/";

        protected readonly object LockObj = new();

        protected CancellationToken CancelToken;

        protected List<TimeSpan> TimePerLoop;
        protected List<TimeSpan> TimePerQuery;
        protected long[] LastReachedIds;

        protected int LastId, Pages, CurrentId, UtilizedThreads;

        protected bool AllThreadsUsed;

        protected Timer Timer;
        protected DateTime Timestamp;

        protected BaseMigrate(int startId, int? endId)
        {
            this.startChunk = startId / ChunkSize;
            this.CurrentId = startId;
            this.LastId = endId ?? 0;

            this.Timer = new Timer(10 * 1000) // 10 seconds
            {
                AutoReset = true,
                Enabled = false,
            };
            this.Timer.Stop();

            this.Timer.Elapsed += this.OnTimerElapsed;
        }

        public void BeforeInit()
        {
            this.currentRunPath = this.GetType().Name + " " + DateTime.Now.ToString("yyyy.MM.dd HH.mm.ss");

            _ = Directory.CreateDirectory(this.currentRunPath);

            this.Timestamp = DateTime.Now;

            Console.Clear();
            Console.WriteLine("Start time: " + this.Timestamp.ToString("T"));

            this.LastReachedIds = new long[ThreadCount];
        }

        public abstract void Init();

        public void AfterInit()
        {
            this.Pages = (int)Math.Ceiling(this.LastId / (double)ChunkSize);

            this.TimePerLoop = new List<TimeSpan>(this.Pages + 1);
            this.TimePerQuery = new List<TimeSpan>(this.Pages + 1);

            this.WriteStatus();
        }

        public virtual async Task Run()
        {
            this.Timer.Start();

            CancellationTokenSource cts = new();
            CancellationTokenSource finalCTS = new();

            this.CancelToken = cts.Token;

#pragma warning disable CS4014

            Task.Factory.StartNew(async () =>
            {
                while (!this.hasFinished)
                {
                    await File.WriteAllTextAsync(this.currentRunPath + "/LastIds.json", this.ToJson(this.LastReachedIds));
                    await Task.Delay(10);
                }
            }, TaskCreationOptions.LongRunning);

            Task.Factory.StartNew(async () =>
            {
                Task[] tasks = new Task[ThreadCount];

                for (int i = 0; i < ThreadCount; i++)
                {
                    tasks[i] = Task.CompletedTask;
                }

                for (int i = 0; i < ThreadCount; i++)
                {
                    tasks[i] = Task.Factory.StartNew(() => this.ThreadLoop(i), finalCTS.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

                    await Task.Delay(TimeToNextThread);
                }

                this.AllThreadsUsed = true;

                await Task.WhenAll(tasks);

                this.Timer?.Stop();

                this.OnFinish();

                if (!cts.IsCancellationRequested)
                {
                    Console.WriteLine("Task has finished, type 'end' to return to selection mode");
                }
            }, TaskCreationOptions.LongRunning);

#pragma warning restore CS4014

            while (Console.ReadLine()?.ToLower().Trim() != "end")
            {
            }

            cts.Cancel();

            this.Timer?.Stop();

            DateTime startTime = DateTime.Now;

            while (this.UtilizedThreads > 0)
            {
                Console.Clear();
                Console.WriteLine("Tasks left to cancel " + this.UtilizedThreads);

                await Task.Delay(1000);

                if (DateTime.Now - startTime > TimeSpan.FromMinutes(10))
                {
                    finalCTS.Cancel();
                    this.Timer?.Stop();

                    this.OnFinish();
                    Console.WriteLine("Press any key to return to selection mode.");
                    Console.ReadKey(false);

                    return;
                }
            }

            if (!this.hasFinished)
            {
                Console.Clear();
                Console.WriteLine("All tasks finished, please wait");

                while (!this.hasFinished)
                {
                    await Task.Delay(1000);
                }
            }

            Console.WriteLine("Press any key to return to selection mode.");
            Console.ReadKey(false);
        }

        protected void OnFinish()
        {
            if (this.hasFinished)
            {
                return;
            }

            this.WriteStatus();
            Console.WriteLine("Full time taken: " + (DateTime.Now - this.Timestamp).Humanize(4, true));

            if (this.exceptionIndex > 0)
            {
                Console.WriteLine("Exceptions encountered: " + this.exceptionIndex);
                Console.WriteLine($"Locate the folder {this.currentRunPath} to find the exception log files");
            }

            File.WriteAllText(
                this.currentRunPath + "/Result.json",
                this.ToJson(new
                {
                    ThreadCount,
                    TimeToNextThread,
                    ChunkSize,
                    this.LastReachedIds,
                    this.Pages,
                    this.LastId,
                    LastCheckedChunkId = this.CurrentId,
                    LastCheckedPage = this.CurrentId / ChunkSize,
                    ExceptionCount = this.exceptionIndex,
                    StartTime = this.Timestamp,
                    TimeTaken = DateTime.Now - this.Timestamp,
                    AverageExecutionTime = this.TimePerLoop.Count > 0
                        ? TimeSpan.FromTicks((long)Math.Floor(this.TimePerLoop.Average(f => f.Ticks)))
                        : TimeSpan.Zero,
                    MaxExecutionTime = this.TimePerLoop.Count > 0
                        ? TimeSpan.FromTicks(this.TimePerLoop.Max(f => f.Ticks))
                        : TimeSpan.Zero,
                    MinExecutionTime = this.TimePerLoop.Count > 0
                        ? TimeSpan.FromTicks(this.TimePerLoop.Min(f => f.Ticks))
                        : TimeSpan.Zero,
                    AverageQueryTime = this.TimePerQuery.Count > 0
                        ? TimeSpan.FromTicks((long)Math.Floor(this.TimePerQuery.Average(f => f.Ticks)))
                        : TimeSpan.Zero,
                    MaxQueryTime = this.TimePerQuery.Count > 0
                        ? TimeSpan.FromTicks(this.TimePerQuery.Max(f => f.Ticks))
                        : TimeSpan.Zero,
                    MinQueryTime = this.TimePerQuery.Count > 0
                        ? TimeSpan.FromTicks(this.TimePerQuery.Min(f => f.Ticks))
                        : TimeSpan.Zero,
                    LoopTimes = this.TimePerLoop,
                    QueryTimes = this.TimePerQuery,
                })
            );

            this.hasFinished = true;
        }

        protected abstract void ThreadLoop(int loopIndex);

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.WriteStatus();
        }

        private void WriteStatus()
        {
            int currentPage = (this.CurrentId / ChunkSize) - this.UtilizedThreads;
            int actualCurrentPage = currentPage - this.startChunk;

            TimeSpan timeTaken = DateTime.Now - this.Timestamp;

            TimeSpan timeLeft = this.AllThreadsUsed && this.UtilizedThreads > 0
                ? TimeSpan.FromSeconds(timeTaken.TotalSeconds / actualCurrentPage * (this.Pages - currentPage))
                : TimeSpan.Zero;

            TimeSpan averageExecutionTime = TimeSpan.Zero;
            TimeSpan maxExecutionTime = TimeSpan.Zero;
            TimeSpan minExecutionTime = TimeSpan.Zero;

            TimeSpan averageQueryTime = TimeSpan.Zero;
            TimeSpan maxQueryTime = TimeSpan.Zero;
            TimeSpan minQueryTime = TimeSpan.Zero;

            if (this.TimePerLoop.Count > 0)
            {
                averageExecutionTime = TimeSpan.FromTicks((long)Math.Floor(this.TimePerLoop.Average(f => f.Ticks)));
                maxExecutionTime = TimeSpan.FromTicks(this.TimePerLoop.Max(f => f.Ticks));
                minExecutionTime = TimeSpan.FromTicks(this.TimePerLoop.Min(f => f.Ticks));

                averageQueryTime = TimeSpan.FromTicks((long)Math.Floor(this.TimePerLoop.Average(f => f.Ticks)));
                maxQueryTime = TimeSpan.FromTicks(this.TimePerLoop.Max(f => f.Ticks));
                minQueryTime = TimeSpan.FromTicks(this.TimePerLoop.Min(f => f.Ticks));
            }

            int currentPageLength = this.Pages.ToString().Length - currentPage.ToString().Length;

            string currentPageSpaces = new(' ', currentPageLength > 0 ? currentPageLength : 0);

            string extraSpace = new(' ', 20);

            string result = "Start time: " + this.Timestamp.ToString("T") + extraSpace
                + "\n" + currentPage + currentPageSpaces + " / " + this.Pages + extraSpace
                + "\nEstimated time left: " + (this.AllThreadsUsed ? timeLeft.Humanize(2, true) : "Estimating...") + extraSpace
                + "\nTime taken: " + timeTaken.Humanize(3, true) + extraSpace
                + "\n\nAverage execution time: " + averageExecutionTime.Humanize(2, true) + extraSpace
                + "\nMax execution time: " + maxExecutionTime.Humanize(2, true) + extraSpace
                + "\nMin execution time: " + minExecutionTime.Humanize(2, true) + extraSpace
                + "\n\nAverage query time: " + averageQueryTime.Humanize(2, true) + extraSpace
                + "\nMax query time: " + maxQueryTime.Humanize(2, true) + extraSpace
                + "\nMin query time: " + minQueryTime.Humanize(2, true) + extraSpace;

            Console.Clear();
            Console.WriteLine(result);
        }

        protected void WriteException(Exception ex, ChunkModel chunk, long currentFileId)
        {
            File.WriteAllText(
                $"{this.currentRunPath}/ExceptionLog{this.exceptionIndex}.json",
                this.ToJson(new ExceptionModel
                {
                    Message = ex.Message,
                    StackTrace = ex.StackTrace,
                    Source = ex.Source,
                    ExceptionTypeName = ex.GetType().FullName,
                    Chunk = chunk,
                    CurrentFileId = currentFileId
                })
            );

            this.exceptionIndex++;
        }

        protected string ReadScript(string scriptName)
        {
            return File.ReadAllText(ScriptPath + scriptName + ".sql");
        }

        protected string ComputeFileContentHash(byte[] content)
        {
            SHA256 hasher = SHA256.Create();

            byte[] hashBytes = hasher.ComputeHash(content);

            StringBuilder builder = new();
            foreach (byte b in hashBytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }

        protected string ToJson(object obj)
        {
            JsonSerializerOptions jsonSettings = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };

            return JsonSerializer.Serialize(obj, jsonSettings);
        }

        public void Dispose()
        {
            if (this.Timer == null)
            {
                return;
            }

            this.Timer.Elapsed -= this.OnTimerElapsed;
            this.Timer.Stop();
            this.Timer.Dispose();
            this.Timer = null;
        }
    }
}
