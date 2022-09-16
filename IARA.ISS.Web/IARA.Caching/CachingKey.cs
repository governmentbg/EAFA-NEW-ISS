using System;
using System.Threading;
using System.Threading.Tasks;

namespace IARA.Caching
{
    public class CachingKey : IEquatable<CachingKey>, IEquatable<string>
    {
        public CachingKey(CachingSettings settings)
        {
            this.Key = settings.KeyName;
            this.MinutesCached = settings.MinutesToRefresh;
            this.ShouldWaitInitialResult = settings.ShouldWaitInitialResult;
            LastUpdate = DateTime.MinValue;
            this.WaitHandle = new ManualResetEvent(true);
            Padlock = new object();
        }

        public readonly object Padlock;
        public bool IsEntered { get; set; }
        public string Key { get; private set; }
        public DateTime LastUpdate { get; set; }
        public Task Task { get; set; }
        public ManualResetEvent WaitHandle { get; private set; }
        public uint MinutesCached { get; private set; }
        public bool ShouldWaitInitialResult { get; private set; }

        public bool Equals(CachingKey other)
        {
            return Key == other.Key;
        }

        public override bool Equals(object obj)
        {
            return Equals((CachingKey)obj);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }

        public bool Equals(string other)
        {
            return this.Key == other;
        }

        //public static implicit operator CachingKey(string value)
        //{
        //    return new CachingKey(value);
        //}
    }



}
