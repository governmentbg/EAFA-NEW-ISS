using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.Reports;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface IPersonReportsService : IService
    {
        IQueryable<PersonReportDTO> GetAllPersonsReport(PersonsReportFilters filters);

        List<ReportHistoryDTO> GetPeopleHistory(IEnumerable<int> validPeopleIds);

        IQueryable<LegalEntityReportDTO> GetAllLegalEntitiesReport(LegalEntitiesReportFilters filters);

        List<ReportHistoryDTO> GetLegalEntitiesHistory(IEnumerable<int> ids);

        PersonReportInfoDTO GetPersonReport(int id);

        LegalEntityReportInfoDTO GetLegalEntityReport(int id);
    }
}
