using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.FLUXVMSRequests;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces
{
    public interface IFLUXVMSRequestsService : IService
    {
        IQueryable<FLUXVMSRequestDTO> GetAll(FLUXVMSRequestFilters filters);

        FLUXVMSRequestEditDTO Get(int id);

        IQueryable<FluxFlapRequestDTO> GetAllFlapRequests(FluxFlapRequestFilters filters);

        FluxFlapRequestEditDTO GetFlapRequest(int id);

        void AddFlapRequest(FluxFlapRequestEditDTO request);

        SimpleAuditDTO GetFlapRequestAudit(int id);

        List<NomenclatureDTO> GetAgreementTypes();

        List<NomenclatureDTO> GetCoastalParties();

        List<NomenclatureDTO> GetRequestPurposes();

        List<NomenclatureDTO> GetFishingCategories();

        List<NomenclatureDTO> GetFlapQuotaTypes();
    }
}
