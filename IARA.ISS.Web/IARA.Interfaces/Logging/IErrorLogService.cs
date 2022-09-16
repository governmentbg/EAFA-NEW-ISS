using System.Linq;
using IARA.DomainModels.DTOModels.ErrorLog;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface IErrorLogService : IService
    {
        public IQueryable<ErrorLogDTO> GetAll(ErrorLogFilters filters);

    }
}
