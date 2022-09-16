using System;

namespace IARA.Caching
{
    public class CachingSettings<TService, TObject> : CachingSettings
    {
        public CachingSettings(string keyName, Func<TService, TObject> resultAction)
            : base(keyName)
        {
            this.ResultAction = resultAction;
        }

        public Func<TService, TObject> ResultAction { get; private set; }
    }

    public abstract class CachingSettings
    {
        public CachingSettings(string keyName)
        {
            this.KeyName = keyName;
        }

        public string KeyName { get; }
        public bool ShouldWaitInitialResult { get; set; } = true;
        public bool ShouldWaitUpdate { get; set; } = false;
        public uint MinutesToRefresh { get; set; } = 10;
    }
}
