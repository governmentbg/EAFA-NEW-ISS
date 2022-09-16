using System;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces
{
    public interface IService : IDisposable
    {
        SimpleAuditDTO GetSimpleAudit(int id);
    }
}
