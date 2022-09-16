using System.Collections.Generic;
using System.IO;
using System.Linq;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.DomainModels.DTOModels.ShipsRegister.IncreaseCapacity;
using IARA.DomainModels.DTOModels.ShipsRegister.ReduceCapacity;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Excel.Tools.Models;
using IARA.Interfaces.Applications;

namespace IARA.Interfaces
{
    public interface IShipsRegisterService : IService, IRegisterDeliveryService
    {
        IQueryable<ShipRegisterDTO> GetAllShips(ShipsRegisterFilters filters);

        List<NomenclatureDTO> GetShipCatchQuotaNomenclatures(int shipUId);

        ShipRegisterYearlyQuotaDTO GetShipYearlyQuota(int shipCatchQuotaId);

        Stream DownloadShipRegisterExcel(ExcelExporterRequestModel<ShipsRegisterFilters> request);

        IQueryable<ShipRegisterOriginDeclarationFishDTO> GetShipOriginDeclarations(ShipRegisterOriginDeclarationsFilters filters);

        ShipRegisterEditDTO GetShip(int id);

        ShipRegisterEditDTO GetShipFromChangeOfCircumstancesApplication(int applicationId);

        ShipRegisterEditDTO GetShipFromDeregistrationApplication(int applicationId);

        ShipRegisterEditDTO GetShipFromIncreaseCapacityApplication(int applicationId);

        ShipRegisterEditDTO GetShipFromReduceCapacityApplication(int applicationId);

        ShipRegisterEditDTO GetRegisterByApplicationId(int applicationId);

        ShipRegisterEditDTO GetRegisterByChangeOfCircumstancesApplicationId(int applicationId);

        ShipRegisterEditDTO GetRegisterByChangeCapacityApplicationId(int applicationId);

        int AddShip(ShipRegisterEditDTO ship);

        void EditShip(ShipRegisterEditDTO ship, int? applicationId = null, int? uid = null);

        int EditShipRsr(int shipId, bool hasFishingPermit);

        RegixChecksWrapperDTO<ShipRegisterRegixDataDTO> GetShipRegixData(int applicationId);

        ShipRegisterEditDTO GetApplicationDataForRegister(int applicationId);

        ShipRegisterApplicationEditDTO GetShipApplication(int applicationId);

        List<ShipRegisterEventDTO> GetShipEventHistory(int shipId);

        int AddShipApplication(ShipRegisterApplicationEditDTO ship, ApplicationStatusesEnum? nextManualStatus);

        void EditShipApplication(ShipRegisterApplicationEditDTO ship, ApplicationStatusesEnum? manualStatus = null);

        void EditShipApplicationRegixData(ShipRegisterRegixDataDTO ship);

        ShipChangeOfCircumstancesApplicationDTO GetShipChangeOfCircumstancesApplication(int applicationId);

        RegixChecksWrapperDTO<ShipChangeOfCircumstancesRegixDataDTO> GetShipChangeOfCircumstancesRegixData(int applicationId);

        int AddShipChangeOfCircumstancesApplication(ShipChangeOfCircumstancesApplicationDTO application, ApplicationStatusesEnum? manualStatus = null);

        void EditShipChangeOfCircumstancesApplication(ShipChangeOfCircumstancesApplicationDTO application, ApplicationStatusesEnum? manualStatus = null);

        void EditShipChangeOfCircumstancesRegixData(ShipChangeOfCircumstancesRegixDataDTO application);

        void CompleteShipChangeOfCircumstancesApplication(ShipRegisterChangeOfCircumstancesDTO ships);

        ShipDeregistrationApplicationDTO GetShipDeregistrationApplication(int applicationId);

        RegixChecksWrapperDTO<ShipDeregistrationRegixDataDTO> GetShipDeregistrationRegixData(int applicationId);

        int AddShipDeregistrationApplication(ShipDeregistrationApplicationDTO application, ApplicationStatusesEnum? nextManualStatus = null);

        void EditShipDeregistrationApplication(ShipDeregistrationApplicationDTO application, ApplicationStatusesEnum? manualStatus = null);

        void EditShipDeregistrationRegixData(ShipDeregistrationRegixDataDTO application);

        void CompleteShipDeregistrationApplication(ShipRegisterDeregistrationDTO ships);

        void CompleteShipIncreaseCapacityApplication(ShipRegisterIncreaseCapacityDTO ships);

        void CompleteShipReduceCapacityApplication(ShipRegisterReduceCapacityDTO ships);

        SimpleAuditDTO GetShipOwnerSimpleAudit(int id);

        ShipRegisterRegixDataDTO GetApplicationRegixData(int applicationId);

        ShipChangeOfCircumstancesRegixDataDTO GetApplicationChangeOfCircumstancesRegixData(int applicationId);

        ShipDeregistrationRegixDataDTO GetApplicationDeregistrationData(int applicationId);
    }
}
