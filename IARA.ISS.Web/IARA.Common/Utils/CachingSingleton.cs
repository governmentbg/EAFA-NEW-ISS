using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TLTTS.Common.Utils
{
    public class CachingKey : IEquatable<CachingKey>
    {
        public CachingKey(string key, int cachingPeriodHours = CachingSingleton.CACHING_PERIOD_HOURS)
        {
            this.Key = key;
            this.LastUpdate = DateTime.MinValue;
            this.WaitHandle = new ManualResetEvent(true);
            this.Padlock = new object();
            this.CachingPeriodHours = cachingPeriodHours;
        }

        public readonly object Padlock;
        public bool IsEntered { get; set; }
        public string Key { get; private set; }
        public DateTime LastUpdate { get; set; }
        public int CachingPeriodHours { get; set; }
        public Task Task { get; set; }
        public ManualResetEvent WaitHandle { get; private set; }

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
    }

    public sealed class CachingSingleton
    {
        public const int CACHING_PERIOD_HOURS = 24;

        private static CachingSingleton instance;
        private readonly ConcurrentDictionary<CachingKey, IList> cachedObjects;
        private readonly object creationLock = new object();

        private CachingSingleton()
        {
            cachedObjects = new ConcurrentDictionary<CachingKey, IList>();
        }

        public static CachingSingleton Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CachingSingleton();
                }
                return instance;
            }
        }

        public async Task<List<T>> GetCachedObject<T>(Func<IServiceProvider, List<T>> resultAction, IServiceProvider provider, string keyName = null)
        {
            keyName ??= typeof(T).Name;
            CachingKey key = GetOrCreateKey(keyName, out bool created);

            if (IsCachingPeriodEnded(key.LastUpdate, key.CachingPeriodHours))
            {
                await RefreshCache(key, resultAction, provider);
                key.WaitHandle.WaitOne();
            }

            cachedObjects.TryGetValue(key, out IList value);

            return value as List<T>;
        }

        private bool IsCachingPeriodEnded(DateTime lastUpdate, int cachingPeriodHours)
        {
            TimeSpan hours = TimeSpan.FromHours(cachingPeriodHours);
            return (DateTime.Now - lastUpdate) > hours;
        }

        public CachingKey GetOrCreateKey(string keyName, out bool created)
        {
            CachingKey key = cachedObjects.Keys.Where(x => x.Key == keyName).FirstOrDefault();

            if (key == null)
            {
                lock (creationLock)
                {
                    key = cachedObjects.Keys.Where(x => x.Key == keyName).FirstOrDefault();
                    if (key == null)
                    {
                        key = new CachingKey(keyName);
                        cachedObjects.TryAdd(key, null);
                    }
                }

                key = cachedObjects.Keys.Where(x => x.Key == keyName).FirstOrDefault();
                created = true;
                return key;
            }
            else
            {
                created = false;
                return key;
            }
        }

        private Task<bool> RefreshCache<T>(CachingKey key, Func<IServiceProvider, List<T>> resultAction, IServiceProvider provider)
        {
            key.WaitHandle.WaitOne();
            key.WaitHandle.Reset();

            if (IsCachingPeriodEnded(key.LastUpdate, key.CachingPeriodHours))
            {
                key.Task = Task.Factory.StartNew<IList>(() =>
                {
                    return resultAction(provider);
                }).ContinueWith((t) =>
                {
                    if (t.IsCompleted && !t.IsFaulted)
                    {
                        cachedObjects.AddOrUpdate(key, t.Result, (k, v) =>
                                        {
                                            if (v != null)
                                            {
                                                v.Clear();
                                            }

                                            v = null;
                                            return t.Result;
                                        });
                    }

                    key.Task = null;
                    key.LastUpdate = DateTime.Now;
                    key.WaitHandle.Set();
                });
                return Task.FromResult(true);
            }
            else
            {
                return Task.FromResult(false);
            }
        }
    }
}
