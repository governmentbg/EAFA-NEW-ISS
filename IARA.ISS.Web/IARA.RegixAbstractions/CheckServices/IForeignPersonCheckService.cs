using System;
using System.Threading.Tasks;
using IARA.Common;
using IARA.RegixAbstractions.Models;
using TL.RegiXClient.Extended.Models.ForeignPersonIdentity;

namespace IARA.RegixAbstractions.CheckServices
{
    public interface IForeignPersonCheckService
    {
        void AddRegixCheck(RegixContextData<ForeignIdentityInfoRequestType, RegixPersonContext> request, IScopedServiceProvider scopedServiceProvider);
        Task<bool> Enqueue(RegixContextData<ForeignIdentityInfoRequestType, RegixPersonContext> request, uint priority = uint.MaxValue, TimeSpan? timeToDelay = null);
    }
}
