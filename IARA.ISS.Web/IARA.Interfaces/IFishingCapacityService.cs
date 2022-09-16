using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.FishingCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.Duplicates;
using IARA.DomainModels.DTOModels.FishingCapacity.IncreaseCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.ReduceCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.TransferCapacity;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Excel.Tools.Models;
using IARA.Interfaces.Applications;

namespace IARA.Interfaces
{
    public interface IFishingCapacityService : IService, IRegisterDeliveryService
    {
        // Acquired fishing capacity
        AcquiredFishingCapacityDTO GetAcquiredFishingCapacity(int id);

        AcquiredFishingCapacityDTO GetAcquiredFishingCapacityByApplicationId(int applicationId, RecordTypesEnum recordType = RecordTypesEnum.Application);

        // Register
        IQueryable<ShipFishingCapacityDTO> GetAllShipCapacities(FishingCapacityFilters filters);

        void SetShipCapacityHistories(List<ShipFishingCapacityDTO> caps);

        FishingCapacityFreedActionsDTO GetCapacityFreeActionsFromChangeHistory(CapacityChangeHistoryDTO change);

        FishingCapacityFreedActionsRegixDataDTO GetCapacityFreeActionsRegixFromChangeHistory(CapacityChangeHistoryDTO change);

        CapacityChangeHistoryDTO GetCapacityChangeHistory(int applicationId, RecordTypesEnum recordType);

        SimpleAuditDTO GetFishingCapacityHolderSimpleAudit(int id);

        // Capacity certificates
        IQueryable<FishingCapacityCertificateDTO> GetAllCapacityCertificates(FishingCapacityCertificatesFilters filters);

        void SetCapacityCertificateHistory(List<FishingCapacityCertificateDTO> certificates);

        FishingCapacityCertificateEditDTO GetCapacityCertificate(int id);

        void EditCapacityCertificate(FishingCapacityCertificateEditDTO certificate);

        void DeleteCapacityCertificate(int id);

        void UndoDeleteCapacityCertificate(int id);

        SimpleAuditDTO GetFishingCapacityCertificateSimpleAudit(int id);

        List<FishingCapacityCertificateNomenclatureDTO> GetAllCapacityCertificateNomenclatures(int? userId = null);

        Task<byte[]> DownloadFishingCapacityCertificate(int certificateId);

        Stream DownloadFishingCapacityCertificateExcel(ExcelExporterRequestModel<FishingCapacityCertificatesFilters> request);

        // Increase fishing capacity application
        IncreaseFishingCapacityApplicationDTO GetIncreaseFishingCapacityApplication(int id);

        RegixChecksWrapperDTO<IncreaseFishingCapacityRegixDataDTO> GetIncreaseFishingCapacityRegixData(int applicationId);

        IncreaseFishingCapacityRegixDataDTO GetApplicationIncreaseFishingCapacityRegixData(int applicationId);

        IncreaseFishingCapacityDataDTO GetCapacityDataFromIncreaseCapacityApplication(int applicationId);

        int AddIncreaseFishingCapacityApplication(IncreaseFishingCapacityApplicationDTO application, ApplicationStatusesEnum? nextManualStatus);

        void EditIncreaseFishingCapacityApplication(IncreaseFishingCapacityApplicationDTO application, ApplicationStatusesEnum? manualStatus = null);

        void EditIncreaseFishingCapacityRegixData(IncreaseFishingCapacityRegixDataDTO application);

        void CompleteIncreaseFishingCapacityApplication(IncreaseFishingCapacityApplicationDTO application);

        /// <summary>
        /// Добавяне на CapacityChangeHistory при вписване на кораб или увеличаване на капацитет на кораб 
        /// </summary>
        /// <param name="recordType">RecordType на записа</param>
        /// <param name="applicationId">ID на заявлението</param>
        /// <param name="shipId">Корабът, на който се увеличава капацитетът</param>
        /// <param name="tonnageIncrease">С колко се увеличава капацитетът</param>
        /// <param name="powerIncrease">С колко се увеличава мощността</param>
        /// <param name="acquiredCapacity">Осигуреният капацитет</param>
        /// <param name="remainingCapacityActions">Необходими действия с остатъка от капацитета</param>
        void AddIncreaseCapacityChangeHistory(RecordTypesEnum recordType,
                                              int applicationId,
                                              int shipId,
                                              decimal tonnageIncrease,
                                              decimal powerIncrease,
                                              AcquiredFishingCapacityDTO acquiredCapacity,
                                              FishingCapacityFreedActionsDTO remainingCapacityActions);

        /// <summary>
        /// Редактиране на CapacityChangeHistory при вписване на кораб или увеличаване на капацитета на кораб 
        /// </summary>
        /// <param name="applicationId">ID на заявлението</param>
        /// <param name="tonnageIncrease">С колко се увеличава капацитетът</param>
        /// <param name="powerIncrease">С колко се увеличава мощността</param>
        /// <param name="acquiredCapacity">Осигуреният капацитет</param>
        /// <param name="remainingCapacityActions">Необходими действия с остатъка от капацитета</param>
        /// <param name="isActive">Флаг дали осигуреният капацитет се изтрива, или не</param>
        /// <returns>Дали редакцията е извършена, или не</returns>
        bool EditIncreaseCapacityChangeHistory(int applicationId,
                                               decimal tonnageIncrease,
                                               decimal powerIncrease,
                                               AcquiredFishingCapacityDTO acquiredCapacity,
                                               FishingCapacityFreedActionsDTO remainingCapacityActions,
                                               bool isActive = true);

        /// <summary>
        /// Редактиране на RegiX данни на CapacityChangeHistory при вписване на кораб или увеличаване на капацитета на кораб
        /// </summary>
        /// <param name="applicationId"></param>
        /// <param name="remainingCapacityActions"></param>
        public void EditIncreaseCapacityChangeHistory(int applicationId, FishingCapacityFreedActionsRegixDataDTO remainingCapacityActions);

        // Reduce fishing capacity application
        ReduceFishingCapacityApplicationDTO GetReduceFishingCapacityApplication(int id);

        RegixChecksWrapperDTO<ReduceFishingCapacityRegixDataDTO> GetReduceFishingCapacityRegixData(int applicationId);

        ReduceFishingCapacityRegixDataDTO GetApplicationReduceRegixData(int applicationId);

        ReduceFishingCapacityDataDTO GetCapacityDataFromReduceCapacityApplication(int applicationId);

        int AddReduceFishingCapacityApplication(ReduceFishingCapacityApplicationDTO application, ApplicationStatusesEnum? nextManualStatus);

        void EditReduceFishingCapacityApplication(ReduceFishingCapacityApplicationDTO application, ApplicationStatusesEnum? manualStatus = null);

        void EditReduceFishingCapacityRegixData(ReduceFishingCapacityRegixDataDTO application);

        void CompleteReduceFishingCapacityApplication(ReduceFishingCapacityApplicationDTO application);

        /// <summary>
        /// Добавяне на CapacityChangeHistory при отписване на кораб или намаляване на капацитета на кораб
        /// </summary>
        /// <param name="recordType">RecordType на записа</param>
        /// <param name="applicationId">ID на заявлението</param>
        /// <param name="latestShipCapacityId">Актуалният капацитет на кораба</param>
        /// <param name="tonnageDecrease">С колко трябва да бъде намален бруто тонажът</param>
        /// <param name="powerDecrease">С колко трябва да бъде намалена мощността</param>
        /// <param name="remainingCapacityActions">Необходими действия при освобождаване на капацитета</param>
        void AddReduceCapacityChangeHistory(RecordTypesEnum recordType,
                                            int? applicationId,
                                            int latestShipCapacityId,
                                            decimal tonnageDecrease,
                                            decimal powerDecrease,
                                            FishingCapacityFreedActionsDTO remainingCapacityActions);

        /// <summary>
        /// Редактиране на CapacityChangeHistory при отписване на кораб или намаляване на капацитета на кораб
        /// </summary>
        /// <param name="applicationId">ID на заявлението</param>
        /// <param name="tonnageDecrease">С колко трябва да бъде намален бруто тонажът</param>
        /// <param name="powerDecrease">С колко трябва да бъде намалена мощността</param>
        /// <param name="remainingCapacityActions">Необходими действия при освобождаване на капацитета</param>
        void EditReduceCapacityChangeHistory(int applicationId, decimal tonnageDecrease, decimal powerDecrease, FishingCapacityFreedActionsDTO remainingCapacityActions);

        /// <summary>
        /// Редактиране на RegiX данни от CapacityChangeHistory при отписване на кораб или намаляване на капацитета на кораб
        /// </summary>
        /// <param name="applicationId">ID на заявлението</param>
        /// <param name="remainingCapacityActions">Необходими действия при освобождаване на капацитета</param>
        void EditReduceCapacityChangeHistory(int applicationId, FishingCapacityFreedActionsRegixDataDTO remainingCapacityActions);

        // Transfer fishing capacity application
        TransferFishingCapacityApplicationDTO GetTransferFishingCapacityApplication(int id);

        RegixChecksWrapperDTO<TransferFishingCapacityRegixDataDTO> GetTransferFishingCapacityRegixData(int applicationId);

        TransferFishingCapacityRegixDataDTO GetApplicationTransferRegixData(int applicationId);

        int AddTransferFishingCapacityApplication(TransferFishingCapacityApplicationDTO application, ApplicationStatusesEnum? nextManualStatus);

        void EditTransferFishingCapacityApplication(TransferFishingCapacityApplicationDTO application, ApplicationStatusesEnum? manualStatus = null);

        void EditTransferFishingCapacityRegixData(TransferFishingCapacityRegixDataDTO application);

        void CompleteTransferFishingCapacityApplication(TransferFishingCapacityApplicationDTO application);

        // Duplicates
        CapacityCertificateDuplicateApplicationDTO GetCapacityCertificateDuplicateApplication(int applicationId);

        RegixChecksWrapperDTO<CapacityCertificateDuplicateRegixDataDTO> GetCapacityCertificateDuplicateRegixData(int applicationId);

        CapacityCertificateDuplicateRegixDataDTO GetApplicationCapacityCertificateDuplicateRegixData(int applicationId);

        int AddCapacityCertificateDuplicateApplication(CapacityCertificateDuplicateApplicationDTO application, ApplicationStatusesEnum? nextManualStatus);

        void EditCapacityCertificateDuplicateApplication(CapacityCertificateDuplicateApplicationDTO application, ApplicationStatusesEnum? manualStatus = null);

        void EditCapacityCertificateDuplicateRegixData(CapacityCertificateDuplicateRegixDataDTO application);

        void CompleteCapacityCertificateDuplicateApplication(CapacityCertificateDuplicateApplicationDTO application);

        // Maximum capacity
        IQueryable<MaximumFishingCapacityDTO> GetAllMaximumCapacities(MaximumFishingCapacityFilters filters);

        MaximumFishingCapacityEditDTO GetMaximumCapacity(int id);

        LatestMaximumCapacityDTO GetLatestMaximumCapacities();

        int AddMaximumCapacity(MaximumFishingCapacityEditDTO capacity);

        void EditMaximumCapacity(MaximumFishingCapacityEditDTO capacity);

        SimpleAuditDTO GetMaximumCapacitySimpleAudit(int id);

        // Analysis
        FishingCapacityStatisticsDTO GetFishingCapacityStatistics(int year, int month, int day);
    }
}
