using System;
using System.Threading.Tasks;

namespace IARA.Caching
{
    public interface IMemoryCacheService
    {
        bool ForceRefresh<TService, TObject>(string keyName, Func<TService, TObject> resultAction)
            where TService : class
            where TObject : class;
        Task<TObject> GetCachedObject<TService, TObject>(CachingSettings<TService, TObject> cachingSettings)
            where TService : class
            where TObject : class;
    }
}
