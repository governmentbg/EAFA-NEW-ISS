using System.Collections.Generic;
using System.IO;
using System.Linq;
using IARA.DomainModels.DTOModels.CatchQuotas;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Excel.Tools.Models;

namespace IARA.Interfaces
{
    public interface IYearlyQuotasService : IService
    {
        int Add(YearlyQuotaEditDTO entry);
        IQueryable<YearlyQuotaDTO> GetAll(YearlyQuotasFilters filters);
        IQueryable<QuotaHistDTO> GetHistoryForIds(IEnumerable<int> ids);
        YearlyQuotaEditDTO Get(int id);
        YearlyQuotaEditDTO GetLastYearsQuota(int newQuotaId);
        void Edit(YearlyQuotaEditDTO entry);
        void Transfer(int newQuotaId, int oldQuotaId, int transferValue, string basis);
        List<NomenclatureDTO> GetYearlyQuotasForList();
        void Delete(int id);
        void Restore(int id);
        Stream DownloadYearlyQuotaExcel(ExcelExporterRequestModel<YearlyQuotasFilters> request);
    }
}
