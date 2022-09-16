using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.CrossChecks;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface ICrossChecksAdministrationService : IService
    {
        // Cross checks
        IQueryable<CrossCheckDTO> GetAllCrossChecks(CrossChecksFilters filters);

        CrossCheckEditDTO GetCrossCheck(int id);

        int AddCrossCheck(CrossCheckEditDTO crossCheck);

        void EditCrossCheck(CrossCheckEditDTO crossCheck);

        void DeleteCrossCheck(int id);

        void UndoDeleteCrossCheck(int id);

        // Cross check results
        IQueryable<CrossCheckResultDTO> GetAllCrossCheckResults(CrossCheckResultsFilters filters);

        void DeleteCrossCheckResult(int id);

        void UndoDeleteCrossCheckResult(int id);

        void AssignCrossCheckResult(int checkResultId, int userId);

        CrossCheckResolutionEditDTO GetCrossCheckResolution(int checkResultId);

        void EditCrossCheckResolution(CrossCheckResolutionEditDTO resolution);

        bool HasUserAccessToCrossCheckResolution(int checkResultId, int userId);

        SimpleAuditDTO GetCrossCheckResultSimpleAudit(int id);

        // Nomenclatures
        List<NomenclatureDTO> GetAllReportGroups();

        List<NomenclatureDTO> GetCheckResolutionTypes();
    }
}
