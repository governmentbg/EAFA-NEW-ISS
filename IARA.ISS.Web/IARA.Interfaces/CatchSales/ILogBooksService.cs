using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;

namespace IARA.Interfaces.CatchSales
{
    public interface ILogBooksService : IService
    {
        IQueryable<LogBookRegisterDTO> GetAllLogBooks(CatchesAndSalesAdministrationFilters filters, List<LogBookTypesEnum> permittedLogBookTypes);

        IQueryable<LogBookRegisterDTO> GetAllLogBooks(CatchesAndSalesPublicFilters filters, List<LogBookTypesEnum> permittedLogBookTypes, int userId);

        List<CommercialFishingLogBookEditDTO> GetPermitLicenseLogBooks(int permitLicenseId);

        CommercialFishingLogBookEditDTO GetCommercialFishingLogBook(int id);

        LogBookEditDTO GetLogBook(int id);

        LogBookPagesDTO GetLogBookPagesForTable(IEnumerable<int> logBookIds, CatchesAndSalesAdministrationFilters filters, List<LogBookTypesEnum> permittedLogBookTypes);

        LogBookPagesDTO GetLogBookPagesForTable(IEnumerable<int> logBookIds, CatchesAndSalesPublicFilters filters, List<LogBookTypesEnum> permittedLogBookTypes);

        List<LogBookEditDTO> GetBuyerLogBooks(int buyerId);

        List<RangeOverlappingLogBooksDTO> GetOverlappedLogBooks(List<OverlappingLogBooksParameters> ranges);

        List<ShipLogBookPageRegisterDTO> GetShipLogBookPagesAndDeclarations(int logBookId, int? permitLicenseId = null);

        List<AdmissionLogBookPageRegisterDTO> GetAdmissionLogBookPagesAndDeclarations(int logBookId, int? permitLicenseId = null);

        List<TransportationLogBookPageRegisterDTO> GetTransportationLogBookPagesAndDeclarations(int logBookId, int? permitLicenseId = null);

        List<LogBookEditDTO> GetAquacultureFacilityLogBooks(int aquacultureFacilityId);

        BasicLogBookPageDocumentDataDTO GetLogBookPageDocumentData(BasicLogBookPageDocumentParameters parameters);

        List<LogBookNomenclatureDTO> GetLogBookPageDocumentOwnerData(decimal documentNumber, LogBookPageDocumentTypesEnum documentType);

        bool CheckDocumentPageToAddExistance(decimal pageToAdd, int logBookId, LogBookTypesEnum logBookType);

        LogBookPageStatusesEnum? CheckDocumentPageToAddStatus(decimal pageToAdd, LogBookTypesEnum logBookType);

        CommonLogBookPageDataDTO GetCommonLogBookPageDataDTO(CommonLogBookPageDataParameters parameters);

        List<OnBoardCatchRecordFishDTO> GetPreviousTripsOnBoardCatchRecords(int shipId);

        List<LogBookPageProductDTO> GetPossibleProducts(int shipPageId);

        ShipLogBookPageEditDTO GetShipLogBookPage(int id);

        List<ShipLogBookPageEditDTO> GetNewShipLogBookPages(long pageNumber, int logBookId);

        FirstSaleLogBookPageEditDTO GetFirstSaleLogBookPage(int id);

        FirstSaleLogBookPageEditDTO GetNewFirstSaleLogBookPage(int logBookId, int? originDeclarationId, int? transportationDocumentId, int? admissionDocumentId);

        AdmissionLogBookPageEditDTO GetAdmissionLogBookPage(int id);

        AdmissionLogBookPageEditDTO GetNewAdmissionLogBookPage(int logBookId, int? originDeclarationId, int? transportationDocumentId);

        TransportationLogBookPageEditDTO GetTransportationLogBookPage(int id);

        TransportationLogBookPageEditDTO GetNewTransportationLogBookPage(int logBookId, int? originDeclarationId);

        AquacultureLogBookPageEditDTO GetAquacultureLogBookPage(int id);

        AquacultureLogBookPageEditDTO GetNewAquacultureLogBookPage(int logBookId);

        int AddShipLogBookPage(ShipLogBookPageEditDTO page);

        void EditShipLogBookPage(ShipLogBookPageEditDTO page);

        int AddFirstSaleLogBookPage(FirstSaleLogBookPageEditDTO page);

        void EditFirstSaleLogBookPage(FirstSaleLogBookPageEditDTO page);

        int AddAdmissionLogBookPage(AdmissionLogBookPageEditDTO page);

        void EditAdmissionLogBookPage(AdmissionLogBookPageEditDTO page);

        int AddTransportationLogBookPage(TransportationLogBookPageEditDTO page);

        void EditTransportationLogBookPage(TransportationLogBookPageEditDTO page);

        int AddAquacultureLogBookPage(AquacultureLogBookPageEditDTO page);

        void EditAquacultureLogBookPage(AquacultureLogBookPageEditDTO page);

        void AnnulLogBookPage(LogBookPageCancellationReasonDTO reasonData);

        bool CheckOriginDeclarationExistance(string number);

        bool CheckTransportationDocumentExistance(decimal number);

        bool CheckAdmissionDocumentExistance(decimal number);

        SimpleAuditDTO GetShipLogBookPageSimpleAudit(int id);

        SimpleAuditDTO GetCatchRecordSimpleAudit(int id);

        SimpleAuditDTO GetOriginDeclarationFishSimpleAudit(int id);

        SimpleAuditDTO GetFirstSaleLogBookPageSimpleAudit(int id);

        SimpleAuditDTO GetLogBookPageProductAudit(int id);

        SimpleAuditDTO GetAdmissionLogBookPageSimpleAudit(int id);

        SimpleAuditDTO GetTransportationLogBookPageSimpleAudit(int id);

        SimpleAuditDTO GetAquacultureLogBookPageSimpleAudit(int id);
    }
}
