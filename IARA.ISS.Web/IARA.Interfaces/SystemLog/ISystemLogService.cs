
using System.Linq;
using IARA.DomainModels.DTOModels.SystemLog;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface ISystemLogService : IService
    {
        public IQueryable<SystemLogDTO> GetAll(SystemLogFilters filters);

        public SystemLogViewDTO Get(int id);
    }
}
