using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.StatisticalForms;
using IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms;
using IARA.DomainModels.DTOModels.StatisticalForms.FishVessels;
using IARA.DomainModels.DTOModels.StatisticalForms.Reworks;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces.Applications;

namespace IARA.Interfaces
{
    public interface IStatisticalFormsService : IService, IRegisterDeliveryService
    {
        // Register
        IQueryable<StatisticalFormDTO> GetAllStatisticalForms(StatisticalFormsFilters filters, List<StatisticalFormTypesEnum> types, int? userId);

        StatisticalFormAquaFarmEditDTO GetStatisticalFormAquaFarm(int id);

        StatisticalFormReworkEditDTO GetStatisticalFormRework(int id);

        StatisticalFormFishVesselEditDTO GetStatisticalFormFishVessel(int id);

        ApplicationSubmittedByDTO GetUserAsSubmittedBy(int userId, StatisticalFormTypesEnum type);

        StatisticalFormAquaFarmEditDTO GetAquaFarmRegisterByApplicationId(int applicationId);

        StatisticalFormReworkEditDTO GetReworkRegisterByApplicationId(int applicationId);

        StatisticalFormFishVesselEditDTO GetFishVesselRegisterByApplicationId(int applicationId);

        int AddStatisticalFormAquaFarm(StatisticalFormAquaFarmEditDTO form);

        int AddStatisticalFormRework(StatisticalFormReworkEditDTO form);

        int AddStatisticalFormFishVessel(StatisticalFormFishVesselEditDTO form);

        void EditStatisticalFormAquaFarm(StatisticalFormAquaFarmEditDTO form);

        void EditStatisticalFormRework(StatisticalFormReworkEditDTO form);

        void EditStatisticalFormFishVessel(StatisticalFormFishVesselEditDTO form);

        void DeleteStatisticalForm(int id);

        void UndoDeleteStatisticalForm(int id);

        // Aquaculture farms
        StatisticalFormAquaFarmApplicationEditDTO GetStatisticalFormAquaFarmApplication(int applicationId);

        RegixChecksWrapperDTO<StatisticalFormAquaFarmRegixDataDTO> GetStatisticalFormAquaFarmRegixData(int applicationId);

        StatisticalFormAquaFarmRegixDataDTO GetApplicationAquaFarmRegixData(int applicationId);

        StatisticalFormAquaFarmEditDTO GetApplicationAquaFarmDataForRegister(int applicationId);

        int AddStatisticalFormAquaFarmApplication(StatisticalFormAquaFarmApplicationEditDTO form, ApplicationStatusesEnum? nextManualStatus = null);

        void EditStatisticalFormAquaFarmApplication(StatisticalFormAquaFarmApplicationEditDTO form, ApplicationStatusesEnum? manualStatus = null);

        void EditStatisticalFormAquaFarmApplicationRegixData(StatisticalFormAquaFarmRegixDataDTO form);

        StatisticalFormAquacultureDTO GetStatisticalFormAquaculture(int aquacultureId);

        // Reworks
        StatisticalFormReworkApplicationEditDTO GetStatisticalFormReworkApplication(int applicationId);

        RegixChecksWrapperDTO<StatisticalFormReworkRegixDataDTO> GetStatisticalFormReworkRegixData(int applicationId);

        StatisticalFormReworkRegixDataDTO GetApplicationReworkRegixData(int applicationId);

        StatisticalFormReworkEditDTO GetApplicationReworkDataForRegister(int applicationId);

        int AddStatisticalFormReworkApplication(StatisticalFormReworkApplicationEditDTO form, ApplicationStatusesEnum? nextManualStatus = null);

        void EditStatisticalFormReworkApplication(StatisticalFormReworkApplicationEditDTO form, ApplicationStatusesEnum? manualStatus = null);

        void EditStatisticalFormReworkApplicationRegixData(StatisticalFormReworkRegixDataDTO form);

        // Fishing vessels
        StatisticalFormShipDTO GetStatisticalFormShip(int shipId);

        StatisticalFormFishVesselApplicationEditDTO GetStatisticalFormFishVesselApplication(int applicationId);

        RegixChecksWrapperDTO<StatisticalFormFishVesselRegixDataDTO> GetStatisticalFormFishVesselRegixData(int applicationId);

        StatisticalFormFishVesselRegixDataDTO GetApplicationFishVesselRegixData(int applicationId);

        StatisticalFormFishVesselEditDTO GetApplicationFishVesselDataForRegister(int applicationId);

        int AddStatisticalFormFishVesselApplication(StatisticalFormFishVesselApplicationEditDTO form, ApplicationStatusesEnum? nextManualStatus = null);

        void EditStatisticalFormFishVesselApplication(StatisticalFormFishVesselApplicationEditDTO form, ApplicationStatusesEnum? manualStatus = null);

        void EditStatisticalFormFishVesselApplicationRegixData(StatisticalFormFishVesselRegixDataDTO form);
    }
}
