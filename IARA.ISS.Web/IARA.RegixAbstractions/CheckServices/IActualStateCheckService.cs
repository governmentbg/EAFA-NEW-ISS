using System;
using System.Threading.Tasks;
using IARA.Common;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Extended.Models.ActualState;

namespace IARA.RegixAbstractions.CheckServices
{
    public interface IActualStateCheckService
    {
        void AddRegixCheck(RegixContextData<ActualStateRequestType, RegixLegalContext> request, IScopedServiceProvider scopedServiceProvider);
        Task<bool> Enqueue(RegixContextData<ActualStateRequestType, RegixLegalContext> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null);
    }
}
