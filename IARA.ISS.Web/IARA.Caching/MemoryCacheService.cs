using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common;

namespace IARA.Caching
{
    public sealed class MemoryCacheService : IMemoryCacheService
    {
        private readonly ConcurrentDictionary<string, CachingKey> cachedKeys;
        private readonly ConcurrentDictionary<CachingKey, object> cachedObjects;
        private readonly ScopedServiceProviderFactory serviceProviderFactory;

        public MemoryCacheService(ScopedServiceProviderFactory serviceProviderFactory)
        {
            this.cachedObjects = new ConcurrentDictionary<CachingKey, object>();
            this.cachedKeys = new ConcurrentDictionary<string, CachingKey>();
            this.serviceProviderFactory = serviceProviderFactory;
        }

        public bool ForceRefresh<TService, TObject>(string keyName, Func<TService, TObject> resultAction)
            where TObject : class
            where TService : class
        {
            if (ContainsKey(keyName))
            {
                CachingKey cachingKey = cachedKeys[keyName];
                cachingKey.LastUpdate = DateTime.MinValue;
                RefreshValue(cachingKey, resultAction);
                return true;
            }
            else
            {
                return false;
            }
        }

        public Task<TObject> GetCachedObject<TService, TObject>(CachingSettings<TService, TObject> cachingSettings)
            where TObject : class
            where TService : class
        {
            if (ContainsKey(cachingSettings.KeyName))
            {
                CachingKey cachingKey = cachedKeys[cachingSettings.KeyName];

                return CheckPeriod(cachingKey, cachingSettings.ResultAction);
            }
            else
            {
                CachingKey cachingKey = new CachingKey(cachingSettings);

                cachingKey = AddOrGet(cachingKey);

                return CheckPeriod(cachingKey, cachingSettings.ResultAction);
            }
        }

        private CachingKey AddOrGet(CachingKey key)
        {
            if (cachedKeys.TryAdd(key.Key, key))
            {
                cachedObjects.TryAdd(key, null);

                return key;
            }
            else
            {
                key = cachedKeys[key.Key];
            }

            return key;
        }

        private Task<TObject> CheckPeriod<TService, TObject>(CachingKey key, Func<TService, TObject> resultAction)
            where TObject : class
            where TService : class
        {
            if (IsCachingPeriodEnded(key))
            {
                return WaitGetValue(key, resultAction);
            }

            return GetTaskValue<TObject>(key.Key);
        }

        private bool ContainsKey(string key)
        {
            return cachedKeys.ContainsKey(key);
        }

        private Task<TObject> GetTaskValue<TObject>(string key)
             where TObject : class
        {
            return Task.FromResult(GetValue<TObject>(key));
        }

        private TObject GetValue<TObject>(string key)
            where TObject : class
        {
            if (cachedKeys.ContainsKey(key))
            {
                CachingKey cachingKey = cachedKeys[key];

                if (cachedObjects.ContainsKey(cachingKey))
                {
                    return cachedObjects.GetValueOrDefault(cachingKey) as TObject;
                }
            }

            return default;
        }

        private bool IsCachingPeriodEnded(CachingKey key)
        {
            TimeSpan hours = TimeSpan.FromMinutes(key.MinutesCached);
            return DateTime.Now - key.LastUpdate > hours;
        }

        private Task<TObject> RefreshValue<TService, TObject>(CachingKey key, Func<TService, TObject> action)
            where TService : class
            where TObject : class
        {
            if (key.Task == null)
            {
                lock (key.Padlock)
                {
                    if (key.Task == null)
                    {
                        var task = Task.Factory.StartNew(() =>
                          {
                              using (var serviceProvider = serviceProviderFactory.GetServiceProvider())
                              {
                                  TService service = serviceProvider.GetRequiredService<TService>();
                                  return action(service);
                              }
                          }).ContinueWith(t =>
                          {
                              if (t.IsCompletedSuccessfully)
                              {
                                  cachedObjects[key] = t.Result;
                                  key.Task = null;
                                  key.LastUpdate = DateTime.Now;
                                  key.WaitHandle.Set();

                                  return t.Result;
                              }
                              else
                              {
                                  key.Task = null;
                                  key.WaitHandle.Set();
                                  return default;
                              }
                          });

                        key.Task = task;
                        return task;
                    }
                }
            }

            return key.Task as Task<TObject>;
        }

        private bool TryRemove<TObject>(string key, out TObject obj)
            where TObject : class
        {
            if (cachedKeys.TryRemove(key, out CachingKey cachedKey))
            {
                bool result = cachedObjects.TryRemove(cachedKey, out object value);

                obj = value as TObject;

                return result;
            }
            else
            {
                obj = null;
                return false;
            }
        }

        private Task<TObject> WaitGetValue<TService, TObject>(CachingKey key, Func<TService, TObject> resultAction)
            where TObject : class
            where TService : class
        {
            if (key.ShouldWaitInitialResult)
            {
                key.WaitHandle.WaitOne();
                key.WaitHandle.Reset();

                if (key.Task == null)
                {
                    return RefreshValue(cachedKeys[key.Key], resultAction);
                }
                else
                {
                    return GetTaskValue<TObject>(key.Key);
                }
            }
            else
            {
                if (key.Task == null)
                {
                    RefreshValue(cachedKeys[key.Key], resultAction);
                }

                return GetTaskValue<TObject>(key.Key);
            }
        }
    }
}
