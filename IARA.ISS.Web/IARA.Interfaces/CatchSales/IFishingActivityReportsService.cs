using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.FishingActivityReports;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces.CatchSales
{
    public interface IFishingActivityReportsService : IService
    {
        IQueryable<FishingActivityReportDTO> GetAllFishingActivityReports(FishingActivityReportsFilters filters);

        ILookup<string, FishingActivityReportItemDTO> GetAllFishingActivityReportItems(List<string> tripIdentifiers);

        List<FishingActivityReportPageDTO> GetAllFishingActivityReportPages(List<int> reportIds);

        string GetFishingActivityReportJson(int id);
    }
}
