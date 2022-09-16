using System.Linq;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.LegalEntities;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces.Applications;

namespace IARA.Interfaces.Legals
{
    public interface ILegalEntitiesService : IService, IRegisterDeliveryService
    {
        IQueryable<LegalEntityDTO> GetAllLegalEntities(LegalEntitiesFilters filters);

        LegalEntityEditDTO GetLegalEntity(int id);

        LegalEntityEditDTO GetRegisterByApplicationId(int applicationId);

        int AddLegalEntity(LegalEntityEditDTO legalEntity);

        void EditLegalEntity(LegalEntityEditDTO legalEntity);

        RegixChecksWrapperDTO<LegalEntityRegixDataDTO> GetLegalEntityRegixData(int applicationId);

        LegalEntityEditDTO GetApplicationDataForRegister(int applicationId);

        LegalEntityApplicationEditDTO GetLegalEntityApplication(int applicationId);

        int AddLegalEntityApplication(LegalEntityApplicationEditDTO legal, ApplicationStatusesEnum? manualStatus);

        void EditLegalEntityApplication(LegalEntityApplicationEditDTO legal, ApplicationStatusesEnum? manualStatus = null);

        void EditLegalEntityApplicationRegixData(LegalEntityRegixDataDTO legal);

        SimpleAuditDTO GetAuthorizedPersonSimpleAudit(int id);

        AuthorizedPersonDTO GetAuthorizedPersonFromUserId(int userId);

        bool HasUserAccessToLegalEntityFile(int userId, int fileId);

        LegalEntityRegixDataDTO GetApplicationRegixData(int applicationId);
    }
}
