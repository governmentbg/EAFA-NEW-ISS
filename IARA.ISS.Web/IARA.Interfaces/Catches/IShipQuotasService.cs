using System.Collections.Generic;
using System.IO;
using System.Linq;
using IARA.DomainModels.DTOModels.CatchQuotas;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Excel.Tools.Models;

namespace IARA.Interfaces
{
    public interface IShipQuotasService : IService
    {
        int Add(ShipQuotaEditDTO entry);
        IQueryable<ShipQuotaDTO> GetAll(ShipQuotasFilters filters);
        List<QuotaHistDTO> GetHistoryForIds(IEnumerable<int> ids);
        List<NomenclatureDTO> GetShipQuotasForList(int originalShipQuotaId);
        ShipQuotaEditDTO Get(int id);
        bool Edit(ShipQuotaEditDTO entry);
        void Transfer(int newQuotaId, int oldQuotaId, int transferValue, string basis);
        void Delete(int id);
        void Restore(int id);
        Stream DownloadShipQuotaExcel(ExcelExporterRequestModel<ShipQuotasFilters> request);
    }
}
