using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.ApplicationsRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface IApplicationsRegisterService : IService
    {
        IQueryable<ApplicationRegisterDTO> GetAllApplications(ApplicationsRegisterFilters filters,
                                                              int? requesterId = null,
                                                              PageCodeEnum[] permittedCodesReadAll = null,
                                                              List<PageCodeEnum> permittedCodesRead = null);

        IEnumerable<ApplicationsChangeHistoryDTO> GetApplicationChangeHistoryRecordsForTable(IEnumerable<int> applicationIds);

        void DeleteApplication(int id);

        void UndoDeleteApplication(int id);

        List<NomenclatureDTO> GetApplicationTypes(params PageCodeEnum[] pageCode);

        List<NomenclatureDTO> GetApplicationStatuses(params ApplicationHierarchyTypesEnum[] hierarchyTypes);

        List<NomenclatureDTO> GetApplicationSources(params ApplicationHierarchyTypesEnum[] hierarchyTypes);

        SimpleAuditDTO GetApplicationHistorySimpleAudit(int id);
    }
}
