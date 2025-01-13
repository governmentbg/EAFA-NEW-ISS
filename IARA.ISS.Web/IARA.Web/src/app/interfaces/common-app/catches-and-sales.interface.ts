import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { CatchesAndSalesPublicFilters } from '@app/models/generated/filters/CatchesAndSalesPublicFilters';
import { LogBookRegisterDTO } from '@app/models/generated/dtos/LogBookRegisterDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ShipLogBookPageEditDTO } from '@app/models/generated/dtos/ShipLogBookPageEditDTO';
import { FirstSaleLogBookPageEditDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageEditDTO';
import { AdmissionLogBookPageEditDTO } from '@app/models/generated/dtos/AdmissionLogBookPageEditDTO';
import { TransportationLogBookPageEditDTO } from '@app/models/generated/dtos/TransportationLogBookPageEditDTO';
import { AquacultureLogBookPageEditDTO } from '@app/models/generated/dtos/AquacultureLogBookPageEditDTO';
import { PermitLicenseNomenclatureDTO } from '@app/models/generated/dtos/PermitLicenseNomenclatureDTO';
import { LogBookPageCancellationReasonDTO } from '@app/models/generated/dtos/LogBookPageCancellationReasonDTO';
import { CommonLogBookPageDataParameters } from '@app/components/common-app/catches-and-sales/components/add-log-book-page-wizard/models/common-log-book-page-data-parameteres.model';
import { CommonLogBookPageDataDTO } from '@app/models/generated/dtos/CommonLogBookPageDataDTO';
import { CatchesAndSalesAdministrationFilters } from '@app/models/generated/filters/CatchesAndSalesAdministrationFilters';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { FishingGearRegisterNomenclatureDTO } from '@app/models/generated/dtos/FishingGearRegisterNomenclatureDTO';
import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { BasicLogBookPageDocumentParameters } from '@app/components/common-app/catches-and-sales/components/add-ship-page-document-wizard/models/basic-log-book-page-document-parameters.model';
import { BasicLogBookPageDocumentDataDTO } from '@app/models/generated/dtos/BasicLogBookPageDocumentDataDTO';
import { LogBookPageProductDTO } from '@app/models/generated/dtos/LogBookPageProductDTO';
import { LogBookPageDocumentTypesEnum } from '@app/components/common-app/catches-and-sales/enums/log-book-page-document-types.enum';
import { OnBoardCatchRecordFishDTO } from '@app/models/generated/dtos/OnBoardCatchRecordFishDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { LogBookPageEditExceptionDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionDTO';
import { LogBookOwnerNomenclatureDTO } from '@app/models/generated/dtos/LogBookOwnerNomenclatureDTO';
import { LogBookPageNomenclatureDTO } from '@app/models/generated/dtos/LogBookPageNomenclatureDTO';
import { LogBookNomenclatureDTO } from '@app/models/generated/dtos/LogBookNomenclatureDTO';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { LogBookPageFilesDTO } from '@app/models/generated/dtos/LogBookPageFilesDTO';

export interface ICatchesAndSalesService {
    getAllCatchesAndSales(request: GridRequestModel<CatchesAndSalesAdministrationFilters | CatchesAndSalesPublicFilters>): Observable<GridResultModel<LogBookRegisterDTO>>;

    getCommonLogBookPageData(parameters: CommonLogBookPageDataParameters, isEditing: boolean): Observable<CommonLogBookPageDataDTO>;
    getLogBookPageDocumentData(parameters: BasicLogBookPageDocumentParameters): Observable<BasicLogBookPageDocumentDataDTO>;
    getLogBookPageDocumentOwnerData(documentNumber: number, documentType: LogBookPageDocumentTypesEnum): Observable<NomenclatureDTO<number>[]>;
    getAquacultureLogBookPageOwnerData(pageNumber: number): Observable<LogBookNomenclatureDTO[]>;
    getCommonLogBookPageDataByOriginDeclarationNumber(originDeclarationNumber: string): Observable<CommonLogBookPageDataDTO>;

    getPreviousTripOnBoardCatchRecords(shipId: number, currentPageId?: number): Observable<OnBoardCatchRecordFishDTO[]>;

    getPossibleProducts(shipLogBookPageId: number, documentType: LogBookPageDocumentTypesEnum): Observable<LogBookPageProductDTO[]>;
    getPossibleProductsByOriginDeclarationId(originDeclarationId: number, documentType: LogBookPageDocumentTypesEnum): Observable<LogBookPageProductDTO[]>;
    getPossibleProductsByTransportationDocument(transportationDocumentId: number, documentType: LogBookPageDocumentTypesEnum): Observable<LogBookPageProductDTO[]>;
    getPossibleProductsByAdmissionDocument(admissionDocumentId: number, documentType: LogBookPageDocumentTypesEnum): Observable<LogBookPageProductDTO[]>;

    getCommercialFishingLogBook(id: number): Observable<CommercialFishingLogBookEditDTO>;
    getLogBook(id: number): Observable<LogBookEditDTO>;

    getShipLogBookPage(id: number): Observable<ShipLogBookPageEditDTO>;
    getNewShipLogBookPages(pageNumber: number, logBookId: number): Observable<ShipLogBookPageEditDTO[]>;

    getFirstSaleLogBookPage(id: number): Observable<FirstSaleLogBookPageEditDTO>;
    getNewFirstSaleLogBookPage(logBookId: number, originDeclarationId?: number, transportationDocumentId?: number, admissionDocumentId?: number): Observable<FirstSaleLogBookPageEditDTO>;

    getAdmissionLogBookPage(id: number): Observable<AdmissionLogBookPageEditDTO>;
    getNewAdmissionLogBookPage(logBookId: number, originDeclarationId?: number, transportationDocumentId?: number): Observable<AdmissionLogBookPageEditDTO>;

    getTransportationLogBookPage(id: number): Observable<TransportationLogBookPageEditDTO>;
    getNewTransportationLogBookPage(logBookId: number, originDeclarationId?: number): Observable<TransportationLogBookPageEditDTO>;

    getAquacultureLogBookPage(id: number): Observable<AquacultureLogBookPageEditDTO>;
    getNewAquacultureLogBookPage(logBookId: number): Observable<AquacultureLogBookPageEditDTO>;

    getLogBookTypes(): Observable<NomenclatureDTO<number>[]>;
    getAquacultureFacilities(): Observable<NomenclatureDTO<number>[]>;
    getRegisteredBuyersNomenclature(): Observable<NomenclatureDTO<number>[]>;
    getPermitLicenseNomenclatures(): Observable<PermitLicenseNomenclatureDTO[]>;
    getTurbotSizeGroups(): Observable<NomenclatureDTO<number>[]>;
    getFishSizeCategories(): Observable<NomenclatureDTO<number>[]>;
    getCatchStates(): Observable<NomenclatureDTO<number>[]>;
    getUnloadTypes(): Observable<NomenclatureDTO<number>[]>;
    getFishPurposes(): Observable<NomenclatureDTO<number>[]>;
    getFishSizes(): Observable<NomenclatureDTO<number>[]>;
    getCatchTypes(): Observable<NomenclatureDTO<number>[]>;
    getFishingGearsRegister(permitLicenseId: number): Observable<FishingGearRegisterNomenclatureDTO[]>;
    getLogBookPageEditExceptions(): Observable<LogBookPageEditExceptionDTO[]>;
    getShipLogBookPagesByShipId(shipId: number): Observable<LogBookPageNomenclatureDTO[]>;
    getLogBookPageOwners(): Observable<LogBookOwnerNomenclatureDTO[]>;
    getAdmissionPagesByOwnerId(buyerId: number | undefined, legalId: number | undefined, personId: number | undefined): Observable<LogBookPageNomenclatureDTO[]>;
    getTransportationPagesByOwnerId(buyerId: number | undefined, legalId: number | undefined, personId: number | undefined): Observable<LogBookPageNomenclatureDTO[]>;

    addShipLogBookPage(model: ShipLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number>;
    editShipLogBookPage(model: ShipLogBookPageEditDTO): Observable<void>;
    restoreAnnulledShipLogBookPage(logBookPageId: number): Observable<void>;
    editShipLogBookPageNumber(logBookPageId: number, pageNumber: string, hasMissingPagesRangePermission: boolean): Observable<void>;

    addFirstSaleLogBookPage(model: FirstSaleLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number>;
    editFirstSaleLogBookPage(model: FirstSaleLogBookPageEditDTO): Observable<void>;
    restoreAnnulledFirstSaleLogBookPage(logBookPageId: number): Observable<void>;
    editFirstSaleLogBookPageNumber(logBookPageId: number, pageNumber: number, hasMissingPagesRangePermission: boolean): Observable<void>;

    addAdmissionLogBookPage(model: AdmissionLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number>;
    editAdmissionLogBookPage(model: AdmissionLogBookPageEditDTO): Observable<void>;
    restoreAnnulledAdmissionLogBookPage(logBookPageId: number): Observable<void>;
    editAdmissionLogBookPageNumber(logBookPageId: number, pageNumber: number, hasMissingPagesRangePermission: boolean): Observable<void>;

    addTransportationLogBookPage(model: TransportationLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number>;
    editTransportationLogBookPage(model: TransportationLogBookPageEditDTO): Observable<void>;
    restoreAnnulledTransportationLogBookPage(logBookPageId: number): Observable<void>;
    editTransportationLogBookPageNumber(logBookPageId: number, pageNumber: number, hasMissingPagesRangePermission: boolean): Observable<void>;

    addAquacultureLogBookPage(model: AquacultureLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number>;
    editAquacultureLogBookPage(model: AquacultureLogBookPageEditDTO): Observable<void>;
    restoreAnnulledAquacultureLogBookPage(logBookPageId: number): Observable<void>;
    editAquacultureLogBookPageNumber(logBookPageId: number, pageNumber: number, hasMissingPagesRangePermission: boolean): Observable<void>;

    annulLogBookPage(reasonData: LogBookPageCancellationReasonDTO, logBookType: LogBookTypesEnum): Observable<void>;

    getPreviousRelatedLogBookPages(logBookPageId: number): Observable<NomenclatureDTO<number>[]>;
    addRelatedDeclaration(logBookPageId: number, relatedLogBookPageId: number): Observable<void>;

    getShipLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getCatchRecordSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getOriginDeclarationFishSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getFirstSaleLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getLogBookPageProductAudit(id: number): Observable<SimpleAuditDTO>;
    getAdmissionLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getTransportationLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getAquacultureLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getLogBookAudit(id: number): Observable<SimpleAuditDTO>;

    getLogBookPageFiles(id: number, logBookType: LogBookTypesEnum): Observable<FileInfoDTO[]>;
    editLogBookPageFiles(page: LogBookPageFilesDTO): Observable<void>;

    downloadFile(fileId: number, fileName: string): Observable<boolean>;

    getPageStatusTranslation(status: LogBookPageStatusesEnum): string;
}