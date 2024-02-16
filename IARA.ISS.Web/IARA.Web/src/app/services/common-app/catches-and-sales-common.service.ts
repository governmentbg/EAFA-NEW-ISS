import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { LogBookRegisterDTO } from '@app/models/generated/dtos/LogBookRegisterDTO';
import { CatchesAndSalesPublicFilters } from '@app/models/generated/filters/CatchesAndSalesPublicFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { ShipLogBookPageRegisterDTO } from '@app/models/generated/dtos/ShipLogBookPageRegisterDTO';
import { FirstSaleLogBookPageRegisterDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageRegisterDTO';
import { AdmissionLogBookPageRegisterDTO } from '@app/models/generated/dtos/AdmissionLogBookPageRegisterDTO';
import { TransportationLogBookPageRegisterDTO } from '@app/models/generated/dtos/TransportationLogBookPageRegisterDTO';
import { AquacultureLogBookPageRegisterDTO } from '@app/models/generated/dtos/AquacultureLogBookPageRegisterDTO';
import { LogBookPagesDTO } from '@app/models/generated/dtos/LogBookPagesDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ShipLogBookPageEditDTO } from '@app/models/generated/dtos/ShipLogBookPageEditDTO';
import { FirstSaleLogBookPageEditDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageEditDTO';
import { AdmissionLogBookPageEditDTO } from '@app/models/generated/dtos/AdmissionLogBookPageEditDTO';
import { TransportationLogBookPageEditDTO } from '@app/models/generated/dtos/TransportationLogBookPageEditDTO';
import { AquacultureLogBookPageEditDTO } from '@app/models/generated/dtos/AquacultureLogBookPageEditDTO';
import { LogBookPageCancellationReasonDTO } from '@app/models/generated/dtos/LogBookPageCancellationReasonDTO';
import { CommonLogBookPageDataParameters } from '@app/components/common-app/catches-and-sales/components/add-log-book-page-wizard/models/common-log-book-page-data-parameteres.model';
import { CommonLogBookPageDataDTO } from '@app/models/generated/dtos/CommonLogBookPageDataDTO';
import { CatchesAndSalesAdministrationFilters } from '@app/models/generated/filters/CatchesAndSalesAdministrationFilters';
import { LogBookPageStatusesEnum } from '@app/enums/log-book-page-statuses.enum';
import { FishingGearRegisterNomenclatureDTO } from '@app/models/generated/dtos/FishingGearRegisterNomenclatureDTO';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { FishInformationDTO } from '@app/models/generated/dtos/FishInformationDTO';
import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { BasicLogBookPageDocumentParameters } from '@app/components/common-app/catches-and-sales/components/add-ship-page-document-wizard/models/basic-log-book-page-document-parameters.model';
import { BasicLogBookPageDocumentDataDTO } from '@app/models/generated/dtos/BasicLogBookPageDocumentDataDTO';
import { LogBookPageProductDTO } from '@app/models/generated/dtos/LogBookPageProductDTO';
import { LogBookPageDocumentTypesEnum } from '@app/components/common-app/catches-and-sales/enums/log-book-page-document-types.enum';
import { LogBookNomenclatureDTO } from '@app/models/generated/dtos/LogBookNomenclatureDTO';
import { LogBookPagePersonTypesEnum } from '@app/enums/log-book-page-person-types.enum';
import { OnBoardCatchRecordFishDTO } from '@app/models/generated/dtos/OnBoardCatchRecordFishDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { LogBookPageEditExceptionDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionDTO';
import { LogBookOwnerNomenclatureDTO } from '@app/models/generated/dtos/LogBookOwnerNomenclatureDTO';
import { LogBookPageNomenclatureDTO } from '@app/models/generated/dtos/LogBookPageNomenclatureDTO';
import { DatePipe } from '@angular/common';

type FiltersUnion = CatchesAndSalesAdministrationFilters | CatchesAndSalesPublicFilters;

@Injectable({
    providedIn: 'root'
})
export class CatchesAndSalesCommonService {
    private readonly http: RequestService;
    private readonly translate: FuseTranslationLoaderService;
    private readonly permissions: PermissionsService;
    private datePipe: DatePipe;

    public constructor(requestService: RequestService, translate: FuseTranslationLoaderService, permissions: PermissionsService, datePipe: DatePipe) {
        this.http = requestService;
        this.translate = translate;
        this.permissions = permissions;
        this.datePipe = datePipe;
    }

    public getAllCatchesAndSales(area: AreaTypes, controller: string, request: GridRequestModel<FiltersUnion>): Observable<GridResultModel<LogBookRegisterDTO>> {
        type Result = GridResultModel<LogBookRegisterDTO>;
        type Body = GridRequestModel<FiltersUnion>;

        return this.http.post<Result, Body>(area, controller, 'GetAllLogBooks', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        }).pipe(switchMap((entries: Result) => {
            const logBookIds: number[] = entries.records.map((logBook: LogBookRegisterDTO) => {
                return logBook.id!;
            });

            if (logBookIds.length === 0) {
                return of(entries);
            }

            if (this.permissions.has(PermissionsEnum.FishLogBookPageReadAll) || this.permissions.has(PermissionsEnum.FishLogBookPageRead)
                || this.permissions.has(PermissionsEnum.FirstSaleLogBookPageReadAll) || this.permissions.has(PermissionsEnum.FirstSaleLogBookPageRead)
                || this.permissions.has(PermissionsEnum.AdmissionLogBookPageReadAll) || this.permissions.has(PermissionsEnum.AdmissionLogBookPageRead)
                || this.permissions.has(PermissionsEnum.TransportationLogBookPageReadAll) || this.permissions.has(PermissionsEnum.TransportationLogBookPageRead)
                || this.permissions.has(PermissionsEnum.AquacultureLogBookPageReadAll) || this.permissions.has(PermissionsEnum.AquacultureLogBookPageRead)
            ) {
                return this.getLogBookPagesForTable(area, controller, logBookIds, request.filters).pipe(map((logBookPagesData: LogBookPagesDTO) => {
                    for (const shipPage of logBookPagesData.shipPages ?? []) {
                        shipPage.statusName = this.getPageStatusTranslation(shipPage.status!);

                        if (logBookPagesData.unloadingInformation !== null && logBookPagesData.unloadingInformation !== undefined) {
                            const unloadingInformationData: FishInformationDTO[] = logBookPagesData.unloadingInformation!.filter(x => x.logBookPageId === shipPage.id);
                            shipPage.unloadingInformation = unloadingInformationData.map(x => x.fishData).join(';');
                        }

                        const found: LogBookRegisterDTO | undefined = this.findLogBookById(entries.records, shipPage.logBookId!);

                        if (found !== undefined) {
                            if (found.shipPages !== undefined && found.shipPages !== null) {
                                found.shipPages.push(new ShipLogBookPageRegisterDTO(shipPage));
                            }
                            else {
                                found.shipPages = [shipPage];
                            }
                        }
                    }

                    for (const firstSalePage of logBookPagesData.firstSalePages ?? []) {
                        firstSalePage.statusName = this.getPageStatusTranslation(firstSalePage.status!);

                        if (logBookPagesData.firstSaleProductInformation !== null && logBookPagesData.firstSaleProductInformation !== undefined) {
                            const productInformationData: FishInformationDTO[] = logBookPagesData.firstSaleProductInformation.filter(x => x.logBookPageId === firstSalePage.id);
                            firstSalePage.productsInformation = productInformationData.map(x => x.fishData).join(';');
                        }

                        const found: LogBookRegisterDTO | undefined = this.findLogBookById(entries.records, firstSalePage.logBookId!);

                        if (found !== undefined) {
                            if (found.firstSalePages !== undefined && found.firstSalePages !== null) {
                                found.firstSalePages.push(new FirstSaleLogBookPageRegisterDTO(firstSalePage));
                            }
                            else {
                                found.firstSalePages = [firstSalePage];
                            }
                        }
                    }

                    for (const admissionPage of logBookPagesData.admissionPages ?? []) {
                        admissionPage.statusName = this.getPageStatusTranslation(admissionPage.status!);

                        if (logBookPagesData.admissionProductInformation !== null && logBookPagesData.admissionProductInformation !== undefined) {
                            const productInformationData: FishInformationDTO[] = logBookPagesData.admissionProductInformation.filter(x => x.logBookPageId === admissionPage.id);
                            admissionPage.productsInformation = productInformationData.map(x => x.fishData).join(';');
                        }

                        const found: LogBookRegisterDTO | undefined = this.findLogBookById(entries.records, admissionPage.logBookId!);

                        if (found !== undefined) {
                            if (found.admissionPages !== undefined && found.admissionPages !== null) {
                                found.admissionPages.push(new AdmissionLogBookPageRegisterDTO(admissionPage));
                            }
                            else {
                                found.admissionPages = [admissionPage];
                            }
                        }
                    }

                    for (const transportationPage of logBookPagesData.transportationPages ?? []) {
                        transportationPage.statusName = this.getPageStatusTranslation(transportationPage.status!);

                        if (logBookPagesData.transportationProductInformation !== null && logBookPagesData.transportationProductInformation !== undefined) {
                            const productInformationData: FishInformationDTO[] = logBookPagesData.transportationProductInformation.filter(x => x.logBookPageId === transportationPage.id);
                            transportationPage.productsInformation = productInformationData.map(x => x.fishData).join(';');
                        }

                        const found: LogBookRegisterDTO | undefined = this.findLogBookById(entries.records, transportationPage.logBookId!);

                        if (found !== undefined) {
                            if (found.transportationPages !== undefined && found.transportationPages !== null) {
                                found.transportationPages.push(new TransportationLogBookPageRegisterDTO(transportationPage));
                            }
                            else {
                                found.transportationPages = [transportationPage];
                            }
                        }
                    }

                    for (const aquaculturePage of logBookPagesData.aquaculturePages ?? []) {
                        aquaculturePage.statusName = this.getPageStatusTranslation(aquaculturePage.status!);

                        if (logBookPagesData.aquacultureProductInformation !== null && logBookPagesData.aquacultureProductInformation !== undefined) {
                            const productInformationData: FishInformationDTO[] = logBookPagesData.aquacultureProductInformation.filter(x => x.logBookPageId === aquaculturePage.id);
                            aquaculturePage.productionInformation = productInformationData.map(x => x.fishData).join(';');
                        }

                        const found: LogBookRegisterDTO | undefined = this.findLogBookById(entries.records, aquaculturePage.logBookId!);

                        if (found !== undefined) {
                            if (found.aquaculturePages !== undefined && found.aquaculturePages !== null) {
                                found.aquaculturePages.push(new AquacultureLogBookPageRegisterDTO(aquaculturePage));
                            }
                            else {
                                found.aquaculturePages = [aquaculturePage];
                            }
                        }
                    }

                    return entries;
                }));
            }
            else {
                return of(entries);
            }
        }));
    }

    public getCommercialFishingLogBook(area: AreaTypes, controller: string, id: number): Observable<CommercialFishingLogBookEditDTO> {
        type Result = CommercialFishingLogBookEditDTO;
        const params: HttpParams = new HttpParams().append('id', id.toString());

        return this.http.get<Result>(area, controller, 'GetCommercialFishingLogBook', {
            httpParams: params,
            responseTypeCtr: CommercialFishingLogBookEditDTO
        }).pipe(map((logBook: Result) => {
            for (const shipPage of logBook.shipPagesAndDeclarations ?? []) {
                shipPage.statusName = this.getPageStatusTranslation(shipPage.status!);
            }

            for (const admissionPage of logBook.admissionPagesAndDeclarations ?? []) {
                admissionPage.statusName = this.getPageStatusTranslation(admissionPage.status!);
            }

            for (const transportationPage of logBook.transportationPagesAndDeclarations ?? []) {
                transportationPage.statusName = this.getPageStatusTranslation(transportationPage.status!);
            }

            return logBook;
        }));
    }

    public getLogBook(area: AreaTypes, controller: string, id: number): Observable<LogBookEditDTO> {
        type Result = LogBookEditDTO;
        const params: HttpParams = new HttpParams().append('id', id.toString());

        return this.http.get<Result>(area, controller, 'GetLogBook', {
            httpParams: params,
            responseTypeCtr: LogBookEditDTO
        }).pipe(map((logBook: Result) => {
            for (const firstSalePage of logBook.firstSalePages ?? []) {
                firstSalePage.statusName = this.getPageStatusTranslation(firstSalePage.status!);
            }

            for (const admissionPage of logBook.admissionPagesAndDeclarations ?? []) {
                admissionPage.statusName = this.getPageStatusTranslation(admissionPage.status!);
            }

            for (const transportationPage of logBook.transportationPagesAndDeclarations ?? []) {
                transportationPage.statusName = this.getPageStatusTranslation(transportationPage.status!);
            }

            for (const aquaculturePage of logBook.aquaculturePages ?? []) {
                aquaculturePage.statusName = this.getPageStatusTranslation(aquaculturePage.status!);
            }

            return logBook;
        }));;
    }

    public getCommonLogBookPageData(area: AreaTypes, controller: string, parameters: CommonLogBookPageDataParameters): Observable<CommonLogBookPageDataDTO> {
        return this.http.post(area, controller, 'GetCommonLogBookPageData', parameters, {
            responseTypeCtr: CommonLogBookPageDataDTO
        });
    }

    public getLogBookPageDocumentData(area: AreaTypes, controller: string, parameters: BasicLogBookPageDocumentParameters): Observable<BasicLogBookPageDocumentDataDTO> {
        return this.http.post(area, controller, 'GetLogBookPageDocumentData', parameters, {
            responseTypeCtr: BasicLogBookPageDocumentDataDTO
        });
    }

    public getLogBookPageDocumentOwnerData(area: AreaTypes, controller: string, documentNumber: number, documentType: LogBookPageDocumentTypesEnum): Observable<LogBookNomenclatureDTO[]> {
        const params: HttpParams = new HttpParams().append('documentNumber', documentNumber.toString()).append('documentType', documentType.toString());
        return this.http.get<LogBookNomenclatureDTO[]>(area, controller, 'GetLogBookPageDocumentOwnerData', {
            httpParams: params,
            responseTypeCtr: LogBookNomenclatureDTO
        }).pipe(map((entries: LogBookNomenclatureDTO[]) => {
            const logBookOwnerLabel: string = this.translate.getValue('catches-and-sales.add-ship-page-document-wizard-log-book-owner');
            for (const entry of entries) {
                entry.displayName = `${entry.displayName} (${logBookOwnerLabel}: ${entry.ownerName})`;
                if (entry.ownerType !== null && entry.ownerType !== undefined) {
                    const logBookOwnerTypeLabel: string = this.translate.getValue('catches-and-sales.add-ship-page-document-wizard-log-book-owner-type');
                    const logBookOwnerType: string = this.getLogBookOwnerTypeTranslation(entry.ownerType);

                    if (entry.logBookPermitLicenseId !== null && entry.logBookPermitLicenseId !== undefined) {
                        entry.description = `${logBookOwnerTypeLabel}: ${logBookOwnerType}; 
                                             ${this.translate.getValue('catches-and-sales.log-book-page-person-permit-number')}: ${entry.permitLicenseNumber}`;
                    }
                    else {
                        entry.description = `${logBookOwnerTypeLabel}: ${logBookOwnerType}`;
                    }
                }
            }

            return entries;
        }));
    }

    public getPreviousTripsOnBoardCatchRecords(area: AreaTypes, controller: string, shipId: number, currentPageId?: number): Observable<OnBoardCatchRecordFishDTO[]> {
        let params: HttpParams = new HttpParams().append('shipId', shipId.toString());

        if (currentPageId !== null && currentPageId !== undefined) {
            params = params.append('currentPageId', currentPageId.toString());
        }

        return this.http.get(area, controller, 'GetPreviousTripsOnBoardCatchRecords', {
            httpParams: params,
            responseTypeCtr: OnBoardCatchRecordFishDTO
        });
    }

    public getPossibleProducts(area: AreaTypes, controller: string, shipLogBookPageId: number, documentType: LogBookPageDocumentTypesEnum): Observable<LogBookPageProductDTO[]> {
        const params: HttpParams = new HttpParams().append('shipLogBookPageId', shipLogBookPageId.toString()).append('documentType', documentType.toString());
        return this.http.get(area, controller, 'GetPossibleProducts', {
            httpParams: params,
            responseTypeCtr: LogBookPageProductDTO
        });
    }

    public getShipLogBookPage(area: AreaTypes, controller: string, id: number): Observable<ShipLogBookPageEditDTO> {
        const params = new HttpParams().append('id', id.toString());
        return this.http.get(area, controller, 'GetShipLogBookPage', {
            httpParams: params,
            responseTypeCtr: ShipLogBookPageEditDTO
        });
    }

    public getNewShipLogBookPages(area: AreaTypes, controller: string, pageNumber: number, logBookId: number): Observable<ShipLogBookPageEditDTO[]> {
        const params = new HttpParams()
            .append('pageNumber', pageNumber.toString())
            .append('logBookId', logBookId.toString());

        return this.http.get(area, controller, 'GetNewShipLogBookPages', {
            httpParams: params,
            responseTypeCtr: ShipLogBookPageEditDTO
        });
    }

    public getFirstSaleLogBookPage(area: AreaTypes, controller: string, id: number): Observable<FirstSaleLogBookPageEditDTO> {
        const params: HttpParams = new HttpParams().append('id', id.toString());
        return this.http.get(area, controller, 'GetFirstSaleLogBookPage', {
            httpParams: params,
            responseTypeCtr: FirstSaleLogBookPageEditDTO
        });
    }

    public getNewFirstSaleLogBookPage(
        area: AreaTypes,
        controller: string,
        logBookId: number,
        originDeclarationId?: number,
        transportationDocumentId?: number,
        admissionDocumentId?: number
    ): Observable<FirstSaleLogBookPageEditDTO> {
        let params: HttpParams = new HttpParams()
            .append('logBookId', logBookId.toString());

        if (originDeclarationId !== null && originDeclarationId !== undefined) {
            params = params.append('originDeclarationId', originDeclarationId.toString())
        }
        else if (transportationDocumentId !== null && transportationDocumentId !== undefined) {
            params = params.append('transportationDocumentId', transportationDocumentId.toString());
        }
        else if (admissionDocumentId !== null && admissionDocumentId !== undefined) {
            params = params.append('admissionDocumentId', admissionDocumentId.toString());
        }

        return this.http.get(area, controller, 'GetNewFirstSaleLogBookPage', {
            httpParams: params,
            responseTypeCtr: FirstSaleLogBookPageEditDTO
        });
    }

    public getAdmissionLogBookPage(area: AreaTypes, controller: string, id: number): Observable<AdmissionLogBookPageEditDTO> {
        const params: HttpParams = new HttpParams().append('id', id.toString());
        return this.http.get(area, controller, 'GetAdmissionLogBookPage', {
            httpParams: params,
            responseTypeCtr: AdmissionLogBookPageEditDTO
        });
    }

    public getNewAdmissionLogBookPage(area: AreaTypes, controller: string, logBookId: number, originDeclarationId?: number, transportationDocumentId?: number): Observable<AdmissionLogBookPageEditDTO> {
        let params: HttpParams = new HttpParams().append('logBookId', logBookId.toString());

        if (originDeclarationId !== null && originDeclarationId !== undefined) {
            params = params.append('originDeclarationId', originDeclarationId.toString())
        }
        else if (transportationDocumentId !== null && transportationDocumentId !== undefined) {
            params = params.append('transportationDocumentId', transportationDocumentId.toString());
        }

        return this.http.get(area, controller, 'GetNewAdmissionLogBookPage', {
            httpParams: params,
            responseTypeCtr: AdmissionLogBookPageEditDTO
        });
    }

    public getTransportationLogBookPage(area: AreaTypes, controller: string, id: number): Observable<TransportationLogBookPageEditDTO> {
        const params: HttpParams = new HttpParams().append('id', id.toString());
        return this.http.get(area, controller, 'GetTransportationLogBookPage', {
            httpParams: params,
            responseTypeCtr: TransportationLogBookPageEditDTO
        });
    }

    public getNewTransportationLogBookPage(area: AreaTypes, controller: string, logBookId: number, originDeclarationId?: number): Observable<TransportationLogBookPageEditDTO> {
        let params: HttpParams = new HttpParams().append('logBookId', logBookId.toString());

        if (originDeclarationId !== null && originDeclarationId !== undefined) {
            params = params.append('originDeclarationId', originDeclarationId.toString())
        }

        return this.http.get(area, controller, 'GetNewTransportationLogBookPage', {
            httpParams: params,
            responseTypeCtr: TransportationLogBookPageEditDTO
        });
    }

    public getAquacultureLogBookPage(area: AreaTypes, controller: string, id: number): Observable<AquacultureLogBookPageEditDTO> {
        const params: HttpParams = new HttpParams().append('id', id.toString());
        return this.http.get(area, controller, 'GetAquacultureLogBookPage', {
            httpParams: params,
            responseTypeCtr: AquacultureLogBookPageEditDTO
        });
    }

    public getNewAquacultureLogBookPage(area: AreaTypes, controller: string, logBookId: number): Observable<AquacultureLogBookPageEditDTO> {
        const params: HttpParams = new HttpParams().append('logBookId', logBookId.toString());
        return this.http.get(area, controller, 'GetNewAquacultureLogBookPage', {
            httpParams: params,
            responseTypeCtr: AquacultureLogBookPageEditDTO
        });
    }

    public addShipLogBookPage(area: AreaTypes, controller: string, model: ShipLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number> {
        const params: HttpParams = new HttpParams().append('hasMissingPagesRangePermission', hasMissingPagesRangePermission.toString());

        return this.http.post(area, controller, 'AddShipLogBookPage', model, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-add-ship-log-book-page',
            httpParams: params
        });
    }

    public editShipLogBookPage(area: AreaTypes, controller: string, model: ShipLogBookPageEditDTO): Observable<void> {
        return this.http.post(area, controller, 'EditShipLogBookPage', model, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-edit-ship-log-book-page'
        });
    }

    public restoreAnnulledShipLogBookPage(logBookPageId: number): Observable<void> {
        throw new Error("Should not call this method from the common catch and sales service.");
    }

    public addFirstSaleLogBookPage(area: AreaTypes, controller: string, model: FirstSaleLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number> {
        const params: HttpParams = new HttpParams().append('hasMissingPagesRangePermission', hasMissingPagesRangePermission.toString());

        return this.http.post(area, controller, 'AddFirstSaleLogBookPage', model, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-add-first-sale-log-book-page',
            httpParams: params
        });
    }

    public editFirstSaleLogBookPage(area: AreaTypes, controller: string, model: FirstSaleLogBookPageEditDTO): Observable<void> {
        return this.http.post(area, controller, 'EditFirstSaleLogBookPage', model, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-edit-first-sale-log-book-page'
        });
    }

    public restoreAnnulledFirstSaleLogBookPage(logBookPageId: number): Observable<void> {
        throw new Error("Should not call this method from the common catch and sales service.");
    }

    public addAdmissionLogBookPage(area: AreaTypes, controller: string, model: AdmissionLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number> {
        const params: HttpParams = new HttpParams().append('hasMissingPagesRangePermission', hasMissingPagesRangePermission.toString());

        return this.http.post(area, controller, 'AddAdmissionLogBookPage', model, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-add-admission-log-book-page',
            httpParams: params
        });
    }

    public editAdmissionLogBookPage(area: AreaTypes, controller: string, model: AdmissionLogBookPageEditDTO): Observable<void> {
        return this.http.post(area, controller, 'EditAdmissionLogBookPage', model, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-edit-admission-log-book-page'
        });
    }

    public restoreAnnulledAdmissionLogBookPage(logBookPageId: number): Observable<void> {
        throw new Error("Should not call this method from the common catch and sales service.");
    }

    public addTransportationLogBookPage(area: AreaTypes, controller: string, model: TransportationLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number> {
        const params: HttpParams = new HttpParams().append('hasMissingPagesRangePermission', hasMissingPagesRangePermission.toString());

        return this.http.post(area, controller, 'AddTransportationLogBookPage', model, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-add-transportation-log-book-page',
            httpParams: params
        });
    }

    public editTransportationLogBookPage(area: AreaTypes, controller: string, model: TransportationLogBookPageEditDTO): Observable<void> {
        return this.http.post(area, controller, 'EditTransportationLogBookPage', model, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-edit-transportation-log-book-page'
        });
    }

    public restoreAnnulledTransportationLogBookPage(logBookPageId: number): Observable<void> {
        throw new Error("Should not call this method from the common catch and sales service.");
    }

    public addAquacultureLogBookPage(area: AreaTypes, controller: string, model: AquacultureLogBookPageEditDTO, hasMissingPagesRangePermission: boolean): Observable<number> {
        const params: HttpParams = new HttpParams().append('hasMissingPagesRangePermission', hasMissingPagesRangePermission.toString());

        return this.http.post(area, controller, 'AddAquacultureLogBookPage', model, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-add-aquaculture-log-book-page',
            httpParams: params
        });
    }

    public editAquacultureLogBookPage(area: AreaTypes, controller: string, model: AquacultureLogBookPageEditDTO): Observable<void> {
        return this.http.post(area, controller, 'EditAquacultureLogBookPage', model, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-edit-aquaculture-log-book-page'
        });
    }

    public restoreAnnulledAquacultureLogBookPage(logBookPageId: number): Observable<void> {
        throw new Error("Should not call this method from the common catch and sales service.");
    }

    public annulLogBookPage(area: AreaTypes, controller: string, reasonData: LogBookPageCancellationReasonDTO, logBookType: LogBookTypesEnum): Observable<void> {
        let serviceName: string = '';

        switch (logBookType) {
            case LogBookTypesEnum.Ship:
                serviceName = 'AnnulShipLogBookPage';
                break;
            case LogBookTypesEnum.FirstSale:
                serviceName = 'AnnulFirstSaleLogBookPage';
                break;
            case LogBookTypesEnum.Admission:
                serviceName = 'AnnulAdmissionLogBookPage';
                break;
            case LogBookTypesEnum.Transportation:
                serviceName = 'AnnulTransportationLogBookPage';
                break;
            case LogBookTypesEnum.Aquaculture:
                serviceName = 'AnnulAquacultureLogBookPage';
                break;
            default: throw new Error(`Unknown log book type enum: ${LogBookTypesEnum[logBookType]}`);
        }

        return this.http.post(area, controller, serviceName, reasonData, {
            successMessage: 'succ-log-book-page-annulment'
        });
    }

    public downloadFile(area: AreaTypes, controller: string, fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.http.download(area, controller, 'DownloadFile', fileName, { httpParams: params });
    }

    // Nomenclatures

    public getLogBookTypes(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        return this.http.get(area, controller, 'GetLogBookTypes', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getRegisteredBuyersNomenclature(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        return this.http.get(area, controller, 'GetRegisteredBuyersNomenclature', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getAquacultureFacilities(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        return this.http.get<NomenclatureDTO<number>[]>(area, controller, 'GetAquacultureFacilitiesNomenclature', {
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((aquacultures: NomenclatureDTO<number>[]) => {
            for (const aqua of aquacultures) {
                aqua.description = aqua.description!.replace('{UROR}', this.translate.getValue('aquacultures.aquaculture-dropdown-uror'));
                aqua.description = aqua.description!.replace('{REGNUM}', this.translate.getValue('aquacultures.aquaculture-dropdown-reg-num'));
            }
            return aquacultures;
        }));
    }

    public getTurbotSizeGroups(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        return this.http.get(area, controller, 'GetTurbotSizeGroups', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getFishSizeCategories(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        return this.http.get(area, controller, 'GetFishSizeCategories', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getCatchStates(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        return this.http.get(area, controller, 'GetCatchStates', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getUnloadTypes(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        return this.http.get(area, controller, 'GetUnloadTypes', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getFishPurposes(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        return this.http.get(area, controller, 'GetFishPurposes', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getFishSizes(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        type Result = NomenclatureDTO<number>[];

        return this.http.get<Result>(area, controller, 'GetFishSizes', {
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((entries: Result) => {
            for (const entry of entries) {
                entry.displayName = `${entry.code} - ${entry.displayName}`;
            }

            return entries;
        }));
    }

    public getCatchTypes(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        type Result = NomenclatureDTO<number>[];

        return this.http.get<Result>(area, controller, 'GetCatchTypes', {
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((entries: Result) => {
            for (const entry of entries) {
                entry.displayName = `${entry.code} - ${entry.displayName}`;
            }

            return entries;
        }));
    }

    public getFishingGearsRegister(area: AreaTypes, controller: string, permitLicenseId: number): Observable<FishingGearRegisterNomenclatureDTO[]> {
        const params: HttpParams = new HttpParams().append('permitLicenseId', permitLicenseId.toString());

        return this.http.get<FishingGearRegisterNomenclatureDTO[]>(area, controller, 'GetFishingGearsRegister', {
            httpParams: params,
            responseTypeCtr: FishingGearRegisterNomenclatureDTO
        }).pipe(map((fishingGearsRegister: FishingGearRegisterNomenclatureDTO[]) => {
            for (const fishingGearRegister of fishingGearsRegister) {
                fishingGearRegister.displayName = `${fishingGearRegister.code} - ${fishingGearRegister.displayName}`;

                if (fishingGearRegister.netEyeSize !== null && fishingGearRegister.netEyeSize !== undefined) {
                    fishingGearRegister.description = `${this.translate.getValue('common.fishing-gear-register-net-eye-size')}: ${fishingGearRegister.netEyeSize}`;
                }
            }

            return fishingGearsRegister;
        }));
    }

    public getLogBookPageEditExceptions(area: AreaTypes, controller: string): Observable<LogBookPageEditExceptionDTO[]> {
        return this.http.get(area, controller, 'GetLogBookPageEditExceptions', {
            responseTypeCtr: LogBookPageEditExceptionDTO
        });
    }

    public getShipLogBookPagesByShipId(area: AreaTypes, controller: string, shipId: number): Observable<LogBookPageNomenclatureDTO[]> {
        const params: HttpParams = new HttpParams().append('shipId', shipId.toString());

        return this.http.get<LogBookPageNomenclatureDTO[]>(area, controller, 'GetShipLogBookPagesByShipId', {
            httpParams: params,
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((entries: LogBookPageNomenclatureDTO[]) => {
            const pageNum: string = this.translate.getValue('catches-and-sales.ship-page');
            const pageDate: string = this.translate.getValue('catches-and-sales.common-log-book-page-data-origin-declaration-date');

            for (const entry of entries) {
                entry.displayName = `${pageNum}: ${entry.displayName} | ${pageDate}: ${this.datePipe.transform(entry.fillDate, 'dd.MM.yyyy')}`;
            }

            return entries;
        }));
    }

    public getLogBookPageOwners(area: AreaTypes, controller: string): Observable<LogBookOwnerNomenclatureDTO[]> {
        return this.http.get<LogBookOwnerNomenclatureDTO[]>(area, controller, 'GetLogBookPageOwners', {
            responseTypeCtr: LogBookNomenclatureDTO
        }).pipe(map((entries: LogBookOwnerNomenclatureDTO[]) => {
            for (const entry of entries) {
                const logBookOwnerTypeLabel: string = this.translate.getValue('catches-and-sales.add-ship-page-document-wizard-log-book-owner-type');
                const logBookOwnerType: string = this.getLogBookOwnerTypeTranslation(entry.ownerType!);

                entry.displayName = `${entry.ownerName} | ${entry.logBookNumber}`;
                entry.description = `${logBookOwnerTypeLabel}: ${logBookOwnerType}`;
            }

            return entries;
        }));
    }

    public getAdmissionPagesByOwnerId(area: AreaTypes, controller: string, buyerId: number | undefined, legalId: number | undefined, personId: number | undefined): Observable<LogBookPageNomenclatureDTO[]> {
        let params: HttpParams = new HttpParams();

        if (buyerId !== null && buyerId !== undefined) {
            params = params.append('buyerId', buyerId.toString());
        }

        if (legalId !== null && legalId !== undefined) {
            params = params.append('legalId', legalId.toString());
        }

        if (personId !== null && personId !== undefined) {
            params = params.append('personId', personId.toString());
        }

        return this.http.get<LogBookPageNomenclatureDTO[]>(area, controller, 'GetAdmissionPagesByOwnerId', {
            httpParams: params,
            responseTypeCtr: LogBookPageNomenclatureDTO
        }).pipe(map((entries: LogBookPageNomenclatureDTO[]) => {
            const pageNum: string = this.translate.getValue('catches-and-sales.admission-page');
            const pageDate: string = this.translate.getValue('admission-handover-date');

            for (const entry of entries) {
                entry.displayName = `${pageNum}: ${entry.displayName} | ${pageDate}: ${this.datePipe.transform(entry.fillDate, 'dd.MM.yyyy')}`;
            }

            return entries;
        }));
    }

    public getTransportationPagesByOwnerId(area: AreaTypes, controller: string, buyerId: number | undefined, legalId: number | undefined, personId: number | undefined): Observable<LogBookPageNomenclatureDTO[]> {
        let params: HttpParams = new HttpParams();

        if (buyerId !== null && buyerId !== undefined) {
            params = params.append('buyerId', buyerId.toString());
        }

        if (legalId !== null && legalId !== undefined) {
            params = params.append('legalId', legalId.toString());
        }

        if (personId !== null && personId !== undefined) {
            params = params.append('personId', personId.toString());
        }

        return this.http.get<LogBookPageNomenclatureDTO[]>(area, controller, 'GetTransportationPagesByOwnerId', {
            httpParams: params,
            responseTypeCtr: LogBookPageNomenclatureDTO
        }).pipe(map((entries: LogBookPageNomenclatureDTO[]) => {
            const pageNum: string = this.translate.getValue('catches-and-sales.transportation-page');
            const pageDate: string = this.translate.getValue('catches-and-sales.transportation-loading-date');

            for (const entry of entries) {
                entry.displayName = `${pageNum}: ${entry.displayName} | ${pageDate}: ${this.datePipe.transform(entry.fillDate, 'dd.MM.yyyy')}`; 
            }

            return entries;
        }));
    }

    // Helpers
    public getPageStatusTranslation(status: LogBookPageStatusesEnum): string {
        switch (status) {
            case LogBookPageStatusesEnum.Canceled:
                return this.translate.getValue('catches-and-sales.page-canceled');
            case LogBookPageStatusesEnum.InProgress:
                return this.translate.getValue('catches-and-sales.page-in-progress');
            case LogBookPageStatusesEnum.Missing:
                return this.translate.getValue('catches-and-sales.page-missing');
            case LogBookPageStatusesEnum.Submitted:
                return this.translate.getValue('catches-and-sales.page-submitted');
            default:
                throw new Error('Invalid log book page status: ' + status);
        }
    }

    private getLogBookPagesForTable(area: AreaTypes, controller: string, logBookIDs: number[], filters: FiltersUnion | undefined): Observable<LogBookPagesDTO> {
        const request = new LogBookData({ filters: filters, logBookIds: logBookIDs });

        return this.http.post(area, controller, 'GetLogBookPagesForTable', request, {
            responseTypeCtr: LogBookPagesDTO
        });
    }

    private findLogBookById(records: LogBookRegisterDTO[], logBookId: number): LogBookRegisterDTO | undefined {
        const found = records.find((entry: LogBookRegisterDTO) => {
            return entry.id === logBookId;
        });

        return found;
    }

    private getLogBookOwnerTypeTranslation(ownerType: LogBookPagePersonTypesEnum): string {
        switch (ownerType) {
            case LogBookPagePersonTypesEnum.RegisteredBuyer:
                return this.translate.getValue('catches-and-sales.log-book-page-person-registered-buyer-type');
            case LogBookPagePersonTypesEnum.Person:
                return this.translate.getValue('catches-and-sales.log-book-page-person-person-type');
            case LogBookPagePersonTypesEnum.LegalPerson:
                return this.translate.getValue('catches-and-sales.log-book-page-person-person-legal-type');
        }
    }
}

class LogBookData {
    public filters: FiltersUnion | undefined;
    public logBookIds!: number[];

    public constructor(obj?: Partial<LogBookData>) {
        Object.assign(this, obj);
    }
}