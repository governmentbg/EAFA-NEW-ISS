using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Buyers;
using IARA.DomainModels.DTOModels.Buyers.Termination;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.Buyers.ChangeOfCircumstances;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces.Applications;

namespace IARA.Interfaces
{
    public interface IBuyersService : IService, IRegisterDeliveryService
    {
        // Register
        IQueryable<BuyerDTO> GetAll(BuyersFilters filter);

        BuyerEditDTO GetRegisterEntry(int id);

        int AddRegisterEntry(BuyerEditDTO newFisherDto, bool ignoreLogBookConflicts);

        void EditRegisterEntry(BuyerEditDTO updatedItem, bool ignoreLogBookConflicts);

        void Delete(int id);

        void Restore(int id);

        void UpdateBuyerStatus(int buyerId, CancellationHistoryEntryDTO status, int? applicationId);

        Task<DownloadableFileDTO> GetRegisterFileForDownload(int registerId, BuyerTypesEnum buyerType, bool duplicate = false);

        // Audits
        SimpleAuditDTO GetPremiseUsageDocumentSimpleAudit(int id);

        SimpleAuditDTO GetBuyerLicenseSimpleAudit(int id);

        // Nomenclatures
        IEnumerable<NomenclatureDTO> GetEntityTypes();

        IEnumerable<NomenclatureDTO> GetAllBuyersNomenclatures();

        IEnumerable<NomenclatureDTO> GetAllFirstSaleCentersNomenclatures();

        IEnumerable<NomenclatureDTO> GetBuyerStatuses();

        // Applications
        BuyerEditDTO GetRegisterByApplicationId(int applicationId);

        BuyerEditDTO GetRegisterByChangeOfCircumstancesApplicationId(int applicationId);

        BuyerEditDTO GetEntryByApplicationId(int applicationId);

        BuyerApplicationEditDTO GetApplicationEntry(int id);

        RegixChecksWrapperDTO<BuyerRegixDataDTO> GetRegixData(int applicationId);

        BuyerRegixDataDTO GetApplicationRegixData(int applicationId);

        int AddApplicationEntry(BuyerApplicationEditDTO newFisherApplicationDto, ApplicationStatusesEnum? nextManualStatus = null);

        int EditApplicationEntry(BuyerApplicationEditDTO application, ApplicationStatusesEnum? manualStatus = null);

        ApplicationStatusesEnum EditApplicationRegixData(BuyerRegixDataDTO buyer);

        // Change of circumstances
        BuyerChangeOfCircumstancesApplicationDTO GetBuyerChangeOfCircumstancesApplication(int applicationId);

        RegixChecksWrapperDTO<BuyerChangeOfCircumstancesRegixDataDTO> GetBuyerChangeOfCircumstancesRegixData(int applicationId);

        BuyerChangeOfCircumstancesRegixDataDTO GetBuyerApplicationChangeOfCircumstancesRegixData(int applicationId);

        BuyerChangeOfCircumstancesRegixDataDTO GetFirstSaleCenterApplicationChangeOfCircumstancesRegixData(int applicationId);

        int AddBuyerChangeOfCircumstancesApplication(BuyerChangeOfCircumstancesApplicationDTO model, ApplicationStatusesEnum? nextManualStatus = null);

        void EditBuyerChangeOfCircumstancesApplication(BuyerChangeOfCircumstancesApplicationDTO application, ApplicationStatusesEnum? manualStatus = null);

        void EditBuyerChangeOfCircumstancesRegixData(BuyerChangeOfCircumstancesRegixDataDTO application);

        BuyerEditDTO GetBuyerFromChangeOfCircumstancesApplication(int applicationId);

        void CompleteChangeOfCircumstancesApplication(BuyerEditDTO buyer, bool ignoreLogBookConflicts);

        // Termination
        BuyerTerminationApplicationDTO GetBuyerTerminationApplication(int applicationId);

        RegixChecksWrapperDTO<BuyerTerminationRegixDataDTO> GetBuyerTerminationRegixData(int applicationId);

        BuyerTerminationRegixDataDTO GetBuyerApplicationTerminationRegixData(int applicationId);

        BuyerTerminationRegixDataDTO GetFirstSaleCenterApplicationTerminationRegixData(int applicationId);

        int AddBuyerTerminationApplication(BuyerTerminationApplicationDTO application, ApplicationStatusesEnum? nextManualStatus = null);

        void EditBuyerTerminationApplication(BuyerTerminationApplicationDTO application, ApplicationStatusesEnum? nextManualStatus = null);

        void EditBuyerTerminationRegixData(BuyerTerminationRegixDataDTO application);

        BuyerEditDTO GetBuyerFromTerminationApplication(int applicationId);
    }
}
