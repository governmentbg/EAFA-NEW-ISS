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

export interface ICatchesAndSalesService {
    getAllCatchesAndSales(request: GridRequestModel<CatchesAndSalesAdministrationFilters | CatchesAndSalesPublicFilters>): Observable<GridResultModel<LogBookRegisterDTO>>;

    getCommonLogBookPageData(parameters: CommonLogBookPageDataParameters): Observable<CommonLogBookPageDataDTO>;
    getLogBookPageDocumentData(parameters: BasicLogBookPageDocumentParameters): Observable<BasicLogBookPageDocumentDataDTO>;
    getLogBookPageDocumentOwnerData(documentNumber: number, documentType: LogBookPageDocumentTypesEnum): Observable<NomenclatureDTO<number>[]>;

    getPreviousTripOnBoardCatchRecords(shipId: number): Observable<OnBoardCatchRecordFishDTO[]>;

    getPossibleProducts(shipLogBookPageId: number, documentType: LogBookPageDocumentTypesEnum): Observable<LogBookPageProductDTO[]>;

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

    addShipLogBookPage(model: ShipLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number>;
    editShipLogBookPage(model: ShipLogBookPageEditDTO): Observable<void>;

    addFirstSaleLogBookPage(model: FirstSaleLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number>;
    editFirstSaleLogBookPage(model: FirstSaleLogBookPageEditDTO): Observable<void>;

    addAdmissionLogBookPage(model: AdmissionLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number>;
    editAdmissionLogBookPage(model: AdmissionLogBookPageEditDTO): Observable<void>;

    addTransportationLogBookPage(model: TransportationLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number>;
    editTransportationLogBookPage(model: TransportationLogBookPageEditDTO): Observable<void>;

    addAquacultureLogBookPage(model: AquacultureLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number>;
    editAquacultureLogBookPage(model: AquacultureLogBookPageEditDTO): Observable<void>;

    annulLogBookPage(reasonData: LogBookPageCancellationReasonDTO): Observable<void>;

    getShipLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getCatchRecordSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getOriginDeclarationFishSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getFirstSaleLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getLogBookPageProductAudit(id: number): Observable<SimpleAuditDTO>;
    getAdmissionLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getTransportationLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getAquacultureLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getLogBookAudit(id: number): Observable<SimpleAuditDTO>;

    downloadFile(fileId: number, fileName: string): Observable<boolean>;

    getPageStatusTranslation(status: LogBookPageStatusesEnum): string;
}