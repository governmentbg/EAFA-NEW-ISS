import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '@app/services/common-app/base-audit.service';
import { ICatchesAndSalesService } from '@app/interfaces/common-app/catches-and-sales.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { CatchesAndSalesPublicFilters } from '@app/models/generated/filters/CatchesAndSalesPublicFilters';
import { LogBookRegisterDTO } from '@app/models/generated/dtos/LogBookRegisterDTO';
import { CatchesAndSalesCommonService } from '@app/services/common-app/catches-and-sales-common.service';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ShipLogBookPageEditDTO } from '@app/models/generated/dtos/ShipLogBookPageEditDTO';
import { AdmissionLogBookPageEditDTO } from '@app/models/generated/dtos/AdmissionLogBookPageEditDTO';
import { AquacultureLogBookPageEditDTO } from '@app/models/generated/dtos/AquacultureLogBookPageEditDTO';
import { FirstSaleLogBookPageEditDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageEditDTO';
import { TransportationLogBookPageEditDTO } from '@app/models/generated/dtos/TransportationLogBookPageEditDTO';
import { PermitLicenseNomenclatureDTO } from '@app/models/generated/dtos/PermitLicenseNomenclatureDTO';
import { LogBookPageCancellationReasonDTO } from '@app/models/generated/dtos/LogBookPageCancellationReasonDTO';
import { CommonLogBookPageDataParameters } from '@app/components/common-app/catches-and-sales/components/add-log-book-page-wizard/models/common-log-book-page-data-parameteres.model';
import { CommonLogBookPageDataDTO } from '@app/models/generated/dtos/CommonLogBookPageDataDTO';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { FishingGearRegisterNomenclatureDTO } from '@app/models/generated/dtos/FishingGearRegisterNomenclatureDTO';
import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { BasicLogBookPageDocumentParameters } from '@app/components/common-app/catches-and-sales/components/add-ship-page-document-wizard/models/basic-log-book-page-document-parameters.model';
import { BasicLogBookPageDocumentDataDTO } from '@app/models/generated/dtos/BasicLogBookPageDocumentDataDTO';
import { LogBookPageProductDTO } from '@app/models/generated/dtos/LogBookPageProductDTO';
import { LogBookPageDocumentTypesEnum } from '@app/components/common-app/catches-and-sales/enums/log-book-page-document-types.enum';
import { LogBookNomenclatureDTO } from '@app/models/generated/dtos/LogBookNomenclatureDTO';
import { OnBoardCatchRecordFishDTO } from '@app/models/generated/dtos/OnBoardCatchRecordFishDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { LogBookPageEditExceptionDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionDTO';
import { LogBookOwnerNomenclatureDTO } from '@app/models/generated/dtos/LogBookOwnerNomenclatureDTO';
import { LogBookPageNomenclatureDTO } from '@app/models/generated/dtos/LogBookPageNomenclatureDTO';


@Injectable({
    providedIn: 'root'
})
export class CatchesAndSalesPublicService extends BaseAuditService implements ICatchesAndSalesService {
    protected controller: string = 'CatchesAndSalesPublic';

    private commonService: CatchesAndSalesCommonService;

    public constructor(requestService: RequestService, commonService: CatchesAndSalesCommonService) {
        super(requestService, AreaTypes.Public);
        this.commonService = commonService;
    }

    public getAllCatchesAndSales(request: GridRequestModel<CatchesAndSalesPublicFilters>): Observable<GridResultModel<LogBookRegisterDTO>> {
        return this.commonService.getAllCatchesAndSales(this.area, this.controller, request);
    }

    public getCommercialFishingLogBook(id: number): Observable<CommercialFishingLogBookEditDTO> {
        return this.commonService.getCommercialFishingLogBook(this.area, this.controller, id);
    }

    public getLogBook(id: number): Observable<LogBookEditDTO> {
        return this.commonService.getLogBook(this.area, this.controller, id);
    }

    public getCommonLogBookPageData(parameters: CommonLogBookPageDataParameters): Observable<CommonLogBookPageDataDTO> {
        return this.commonService.getCommonLogBookPageData(this.area, this.controller, parameters);
    }

    public getLogBookPageDocumentData(parameters: BasicLogBookPageDocumentParameters): Observable<BasicLogBookPageDocumentDataDTO> {
        return this.commonService.getLogBookPageDocumentData(this.area, this.controller, parameters);
    }

    public getLogBookPageDocumentOwnerData(documentNumber: number, documentType: LogBookPageDocumentTypesEnum): Observable<LogBookNomenclatureDTO[]> {
        return this.commonService.getLogBookPageDocumentOwnerData(this.area, this.controller, documentNumber, documentType);
    }

    public getPreviousTripOnBoardCatchRecords(shipId: number, currentPageId?: number): Observable<OnBoardCatchRecordFishDTO[]> {
        return this.commonService.getPreviousTripsOnBoardCatchRecords(this.area, this.controller, shipId, currentPageId);
    }

    public getPossibleProducts(shipLogBookPageId: number, documentType: LogBookPageDocumentTypesEnum): Observable<LogBookPageProductDTO[]> {
        return this.commonService.getPossibleProducts(this.area, this.controller, shipLogBookPageId, documentType);
    }

    public getShipLogBookPage(id: number): Observable<ShipLogBookPageEditDTO> {
        return this.commonService.getShipLogBookPage(this.area, this.controller, id);
    }

    public getNewShipLogBookPages(pageNumber: number, logBookId: number): Observable<ShipLogBookPageEditDTO[]> {
        return this.commonService.getNewShipLogBookPages(this.area, this.controller, pageNumber, logBookId);
    }

    public getFirstSaleLogBookPage(id: number): Observable<FirstSaleLogBookPageEditDTO> {
        return this.commonService.getFirstSaleLogBookPage(this.area, this.controller, id);
    }

    public getNewFirstSaleLogBookPage(
        logBookId: number,
        originDeclarationId?: number,
        transportationDocumentId?: number,
        admissionDocumentId?: number
    ): Observable<FirstSaleLogBookPageEditDTO> {
        return this.commonService.getNewFirstSaleLogBookPage(this.area, this.controller, logBookId, originDeclarationId, transportationDocumentId, admissionDocumentId);
    }

    public getAdmissionLogBookPage(id: number): Observable<AdmissionLogBookPageEditDTO> {
        return this.commonService.getAdmissionLogBookPage(this.area, this.controller, id);
    }

    public getNewAdmissionLogBookPage(logBookId: number, originDeclarationId?: number, transportationDocumentId?: number): Observable<AdmissionLogBookPageEditDTO> {
        return this.commonService.getNewAdmissionLogBookPage(this.area, this.controller, logBookId, originDeclarationId, transportationDocumentId);
    }

    public getTransportationLogBookPage(id: number): Observable<TransportationLogBookPageEditDTO> {
        return this.commonService.getTransportationLogBookPage(this.area, this.controller, id);
    }

    public getNewTransportationLogBookPage(logBookId: number, originDeclarationId?: number): Observable<TransportationLogBookPageEditDTO> {
        return this.commonService.getNewTransportationLogBookPage(this.area, this.controller, logBookId, originDeclarationId);
    }

    public getAquacultureLogBookPage(id: number): Observable<AquacultureLogBookPageEditDTO> {
        return this.commonService.getAquacultureLogBookPage(this.area, this.controller, id);
    }

    public getNewAquacultureLogBookPage(logBookId: number): Observable<AquacultureLogBookPageEditDTO> {
        return this.commonService.getNewAquacultureLogBookPage(this.area, this.controller, logBookId);
    }

    public addShipLogBookPage(model: ShipLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number> {
        return this.commonService.addShipLogBookPage(this.area, this.controller, model, hasMissingPagesRangePermission);
    }

    public editShipLogBookPage(model: ShipLogBookPageEditDTO): Observable<void> {
        return this.commonService.editShipLogBookPage(this.area, this.controller, model);
    }

    public restoreAnnulledShipLogBookPage(logBookPageId: number): Observable<void> {
        throw new Error("Should not call this method from the public catch and sales service.");
    }

    public addFirstSaleLogBookPage(model: FirstSaleLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number> {
        return this.commonService.addFirstSaleLogBookPage(this.area, this.controller, model, hasMissingPagesRangePermission);
    }

    public editFirstSaleLogBookPage(model: FirstSaleLogBookPageEditDTO): Observable<void> {
        return this.commonService.editFirstSaleLogBookPage(this.area, this.controller, model);
    }

    public restoreAnnulledFirstSaleLogBookPage(logBookPageId: number): Observable<void> {
        throw new Error("Should not call this method from the public catch and sales service.");
    }

    public addAdmissionLogBookPage(model: AdmissionLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number> {
        return this.commonService.addAdmissionLogBookPage(this.area, this.controller, model, hasMissingPagesRangePermission);
    }

    public editAdmissionLogBookPage(model: AdmissionLogBookPageEditDTO): Observable<void> {
        return this.commonService.editAdmissionLogBookPage(this.area, this.controller, model);
    }

    public restoreAnnulledAdmissionLogBookPage(logBookPageId: number): Observable<void> {
        throw new Error("Should not call this method from the public catch and sales service.");
    }

    public addTransportationLogBookPage(model: TransportationLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number> {
        return this.commonService.addTransportationLogBookPage(this.area, this.controller, model, hasMissingPagesRangePermission);
    }

    public editTransportationLogBookPage(model: TransportationLogBookPageEditDTO): Observable<void> {
        return this.commonService.editTransportationLogBookPage(this.area, this.controller, model);
    }

    public restoreAnnulledTransportationLogBookPage(logBookPageId: number): Observable<void> {
        throw new Error("Should not call this method from the public catch and sales service.");
    }

    public addAquacultureLogBookPage(model: AquacultureLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number> {
        return this.commonService.addAquacultureLogBookPage(this.area, this.controller, model, hasMissingPagesRangePermission);
    }

    public editAquacultureLogBookPage(model: AquacultureLogBookPageEditDTO): Observable<void> {
        return this.commonService.editAquacultureLogBookPage(this.area, this.controller, model);
    }

    public restoreAnnulledAquacultureLogBookPage(logBookPageId: number): Observable<void> {
        throw new Error("Should not call this method from the public catch and sales service.");
    }

    public annulLogBookPage(reasonData: LogBookPageCancellationReasonDTO, logBookType: LogBookTypesEnum): Observable<void> {
        return this.commonService.annulLogBookPage(this.area, this.controller, reasonData, logBookType);
    }

    public getPreviousRelatedLogBookPages(logBookPageId: number): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getPreviousRelatedLogBookPages(this.area, this.controller, logBookPageId);
    }

    public addRelatedDeclaration(logBookPageId: number, relatedLogBookPageId: number): Observable<void> {
        return this.commonService.addRelatedDeclaration(this.area, this.controller, logBookPageId, relatedLogBookPageId);
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        return this.commonService.downloadFile(this.area, this.controller, fileId, fileName);
    }

    // Nomenclatures
    public getLogBookTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getLogBookTypes(this.area, this.controller);
    }

    public getRegisteredBuyersNomenclature(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getRegisteredBuyersNomenclature(this.area, this.controller);
    }

    public getAquacultureFacilities(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getAquacultureFacilities(this.area, this.controller);
    }

    public getPermitLicenseNomenclatures(): Observable<PermitLicenseNomenclatureDTO[]> {
        throw new Error('Method should not be called from public app.');
    }

    public getTurbotSizeGroups(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getTurbotSizeGroups(this.area, this.controller);
    }

    public getFishSizeCategories(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getFishSizeCategories(this.area, this.controller);
    }

    public getCatchStates(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getCatchStates(this.area, this.controller);
    }

    public getUnloadTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getUnloadTypes(this.area, this.controller);
    }

    public getFishPurposes(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getFishPurposes(this.area, this.controller);
    }

    public getFishSizes(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getFishSizes(this.area, this.controller);
    }

    public getCatchTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getCatchTypes(this.area, this.controller);
    }

    public getFishingGearsRegister(permitLicenseId: number): Observable<FishingGearRegisterNomenclatureDTO[]> {
        return this.commonService.getFishingGearsRegister(this.area, this.controller, permitLicenseId);
    }

    public getLogBookPageEditExceptions(): Observable<LogBookPageEditExceptionDTO[]> {
        return this.commonService.getLogBookPageEditExceptions(this.area, this.controller);
    }

    public getShipLogBookPagesByShipId(shipId: number): Observable<LogBookPageNomenclatureDTO[]> {
        return this.commonService.getShipLogBookPagesByShipId(this.area, this.controller, shipId);
    }

    public getLogBookPageOwners(): Observable<LogBookOwnerNomenclatureDTO[]> {
        return this.commonService.getLogBookPageOwners(this.area, this.controller);
    }

    public getAdmissionPagesByOwnerId(buyerId: number | undefined, legalId: number | undefined, personId: number | undefined): Observable<LogBookPageNomenclatureDTO[]> {
        return this.commonService.getAdmissionPagesByOwnerId(this.area, this.controller, buyerId, legalId, personId);
    }

    public getTransportationPagesByOwnerId(buyerId: number | undefined, legalId: number | undefined, personId: number | undefined): Observable<LogBookPageNomenclatureDTO[]> {
        return this.commonService.getTransportationPagesByOwnerId(this.area, this.controller, buyerId, legalId, personId);
    }

    // Simple audit
    public getShipLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('Method should not be called from public app.');
    }

    public getCatchRecordSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('Method should not be called from public app.');
    }

    public getOriginDeclarationFishSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('Method should not be called from public app.');
    }

    public getFirstSaleLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('Method should not be called from public app.');
    }

    public getLogBookPageProductAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('Method should not be called from public app.');
    }

    public getAdmissionLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('Method should not be called from public app.');
    }

    public getTransportationLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('Method should not be called from public app.');
    }

    public getAquacultureLogBookPageSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('Method should not be called from public app.');
    }

    public getLogBookAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('Method should not be called from public app.');
    }

    // Helpers
    public getPageStatusTranslation(status: LogBookPageStatusesEnum): string {
        return this.commonService.getPageStatusTranslation(status);
    }
}
