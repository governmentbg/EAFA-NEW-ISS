using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces.CatchSales
{
    public interface ILogBookPageEditExceptionsService : IService
    {
        IQueryable<LogBookPageEditExceptionRegisterDTO> GetAllLogBookPageEditExceptions(LogBookPageEditExceptionFilters filters);
        LogBookPageEditExceptionEditDTO GetLogBookPageEditException(int id);

        void AddOrEditLogBookPageEditException(LogBookPageEditExceptionEditDTO model);
        void DeleteLogBookPageEditException(int id);
        void RestoreLogBookPageEditException(int id);

        // Nomenclatures

        List<SystemUserNomenclatureDTO> GetAllUsersNomenclature();
        List<LogBookPageEditNomenclatureDTO> GetActiveLogBooksNomenclature(int? logBookPageEditExceptionId);
    }
}
