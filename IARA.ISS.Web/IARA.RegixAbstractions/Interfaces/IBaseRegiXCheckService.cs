using System;
using System.Threading.Tasks;
using IARA.Common;
using IARA.RegixAbstractions.Models;

namespace IARA.RegixAbstractions.Interfaces
{
    public interface IBaseRegiXCheckService<TRequest, TCompare> : IDisposable
        where TRequest : class
    {
        void AddRegixCheck(RegixContextData<TRequest, TCompare> request, IScopedServiceProvider scopedServiceProvider);
        Task<bool> Enqueue(RegixContextData<TRequest, TCompare> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null);
    }
}
