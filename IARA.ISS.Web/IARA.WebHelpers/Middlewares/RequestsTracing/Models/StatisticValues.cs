using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace IARA.WebMiddlewares.RequestsTracing
{
    public sealed class StatisticValues : IEnumerable<DateTime>
    {
        private volatile int count;

        public StatisticValues(DateTime exactTime)
        {
            count = 1;
            LastImpresion = exactTime;
            FirstImpression = exactTime;
            Values = new ConcurrentQueue<DateTime>();
        }

        public static uint MaxRecordsCount { get; set; } = 1000;

        public int Count
        {
            get
            {
                return count;
            }
        }

        public DateTime FirstImpression { get; private set; }
        public DateTime LastImpresion { get; private set; }
        public ConcurrentQueue<DateTime> Values { get; }

        public int Aggregate(DateTime exactTime)
        {
            LastImpresion = exactTime;
            Values.Enqueue(exactTime);
            Interlocked.Increment(ref count);
            CheckQueueMaxLength();
            return count;
        }

        public int RemoveAll(DateTime olderThan)
        {
            if (Values.Count > 0)
            {
                while (Values.TryPeek(out DateTime firstImpression) && firstImpression < olderThan && Values.TryDequeue(out _))
                {
                    Interlocked.Decrement(ref count);
                }

                if (Values.TryPeek(out DateTime result))
                {
                    FirstImpression = result;
                }
            }

            return count;
        }

        private bool CheckQueueMaxLength()
        {
            bool removed = false;
            while (Values.Count > MaxRecordsCount)
            {
                removed = true;
                Values.TryDequeue(out _);
                Interlocked.Decrement(ref count);
            }

            if (Values.TryPeek(out DateTime result))
            {
                FirstImpression = result;
            }

            return removed;
        }

        public IEnumerator<DateTime> GetEnumerator()
        {
            return Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
