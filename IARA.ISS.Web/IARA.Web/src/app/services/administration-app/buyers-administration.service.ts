﻿import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { IBuyersService } from '@app/interfaces/common-app/buyers.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { BuyerApplicationEditDTO } from '@app/models/generated/dtos/BuyerApplicationEditDTO';
import { BuyerDTO } from '@app/models/generated/dtos/BuyerDTO';
import { BuyerEditDTO } from '@app/models/generated/dtos/BuyerEditDTO';
import { BuyerRegixDataDTO } from '@app/models/generated/dtos/BuyerRegixDataDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { BuyersFilters } from '@app/models/generated/filters/BuyersFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { BuyerChangeOfCircumstancesApplicationDTO } from '@app/models/generated/dtos/BuyerChangeOfCircumstancesApplicationDTO';
import { BuyerChangeOfCircumstancesRegixDataDTO } from '@app/models/generated/dtos/BuyerChangeOfCircumstancesRegixDataDTO';
import { CancellationHistoryEntryDTO } from '@app/models/generated/dtos/CancellationHistoryEntryDTO';
import { ApplicationsProcessingService } from './applications-processing.service';
import { ApplicationsRegisterAdministrativeBaseService } from './applications-register-administrative-base.service';
import { BuyerTypesEnum } from '@app/enums/buyer-types.enum';
import { BuyerTerminationApplicationDTO } from '@app/models/generated/dtos/BuyerTerminationApplicationDTO';
import { BuyerTerminationBaseRegixDataDTO } from '@app/models/generated/dtos/BuyerTerminationBaseRegixDataDTO';
import { RegisterDTO } from '@app/models/generated/dtos/RegisterDTO';
import { ILogBookService } from '@app/components/common-app/commercial-fishing/components/edit-log-book/interfaces/log-book.interface';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { LogBookForRenewalDTO } from '@app/models/generated/dtos/LogBookForRenewalDTO';
import { RangeOverlappingLogBooksDTO } from '@app/models/generated/dtos/RangeOverlappingLogBooksDTO';
import { OverlappingLogBooksParameters } from '@app/shared/components/overlapping-log-books/models/overlapping-log-books-parameters.model';
import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { LogBookDetailDTO } from '@app/models/generated/dtos/LogBookDetailDTO';
import { ShipLogBookPageRegisterDTO } from '@app/models/generated/dtos/ShipLogBookPageRegisterDTO';
import { FirstSaleLogBookPageRegisterDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageRegisterDTO';
import { AdmissionLogBookPageRegisterDTO } from '@app/models/generated/dtos/AdmissionLogBookPageRegisterDTO';
import { TransportationLogBookPageRegisterDTO } from '@app/models/generated/dtos/TransportationLogBookPageRegisterDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { CatchesAndSalesCommonService } from '../common-app/catches-and-sales-common.service';

@Injectable({
    providedIn: 'root'
})
export class BuyersAdministrationService extends ApplicationsRegisterAdministrativeBaseService implements IBuyersService, ILogBookService {
    protected controller: string = 'Buyers';
    protected applicationsProcessingController: string = 'ApplicationsProcessing';

    private readonly permissions: PermissionsService;
    private readonly catchesAndSalesCommonService: CatchesAndSalesCommonService;


    public constructor(
        requestService: RequestService,
        applicationsRegisterService: ApplicationsProcessingService,
        permissions: PermissionsService,
        catchesAndSalesCommonService: CatchesAndSalesCommonService
    ) {
        super(requestService, applicationsRegisterService);

        this.permissions = permissions;
        this.catchesAndSalesCommonService = catchesAndSalesCommonService;
    }

    public getAll(request: GridRequestModel<BuyersFilters>): Observable<GridResultModel<BuyerDTO>> {
        type Result = GridResultModel<BuyerDTO>;
        type Body = GridRequestModel<BuyersFilters>;

        return this.requestService.post<Result, Body>(this.area, this.controller, 'GetAll', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        }).pipe(switchMap((entries: Result) => {
            const buyerIds: number[] = entries.records.map((buyer: BuyerDTO) => {
                return buyer.id!;
            });

            if (buyerIds.length === 0) {
                return of(entries);
            }

            if (this.permissions.has(PermissionsEnum.BuyerLogBookRead)) {
                return this.getLogBooksForTableHelper(this.controller, buyerIds, request.filters).pipe(map((logBooks: LogBookDetailDTO[]) => {
                    for (const logBook of logBooks) {
                        const found = entries.records.find((entry: BuyerDTO) => {
                            return entry.id === logBook.registerId;
                        });

                        if (found !== undefined) {
                            if (found.logBooks !== undefined && found.logBooks !== null) {
                                found.logBooks.push(new LogBookDetailDTO(logBook));
                            }
                            else {
                                found.logBooks = [logBook];
                            }
                        }
                    }

                    return entries;
                }))
            }
            else {
                return of(entries);
            }
        }));
    }

    public get(id: number): Observable<BuyerEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get', {
            httpParams: params,
            responseTypeCtr: BuyerEditDTO
        });
    }

    public getRegisterByApplicationId(applicationId: number, pageCode: PageCodeEnum): Observable<BuyerEditDTO> {
        let serviceName: string = '';
        const params = new HttpParams().append('applicationId', applicationId.toString());

        switch (pageCode) {
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter:
                serviceName = 'GetRegisterByApplicationId'; break;
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter:
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter:
                serviceName = 'GetRegisterByChangeOfCircumstancesApplicationId'; break;
        }

        return this.requestService.get(this.area, this.controller, serviceName, {
            httpParams: params,
            responseTypeCtr: BuyerEditDTO
        });
    }

    public add(item: BuyerDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'Add', item, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public addAndDownloadRegister(model: BuyerDTO): Observable<boolean> {
        const registerDto: RegisterDTO<BuyerDTO> = new RegisterDTO<BuyerDTO>({
            dto: model
        });

        return this.requestService.downloadPost(this.area, this.controller, 'AddAndDownloadRegister', '', registerDto, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public edit(item: BuyerDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'Edit', item, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editAndDownloadRegister(model: BuyerDTO): Observable<boolean> {
        const registerDto: RegisterDTO<BuyerDTO> = new RegisterDTO<BuyerDTO>({
            dto: model
        });

        return this.requestService.downloadPost(this.area, this.controller, 'EditAndDownloadRegister', '', registerDto, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public downloadRegister(id: number, buyerType: BuyerTypesEnum): Observable<boolean> {
        const params = new HttpParams()
            .append('buyerId', id.toString())
            .append('buyerType', buyerType.toString());
        return this.requestService.downloadPost(this.area, this.controller, 'DownloadRegister', '', undefined, { httpParams: params });
    }

    public delete(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'Delete', { httpParams: params });
    }

    public restore(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDelete', null, { httpParams: params });
    }

    // log books

    public getLogBooksForTable(permitLicenseId: number): Observable<CommercialFishingLogBookEditDTO[]> {
        throw new Error('Not implemented funcionality for buyer log book pages');
    }

    public getLogBook(logBookId: number): Observable<LogBookEditDTO> {
        const params: HttpParams = new HttpParams().append('logBookId', logBookId.toString());

        return this.requestService.get<LogBookEditDTO>(this.area, this.controller, 'GetLogBook', {
            httpParams: params,
            responseTypeCtr: LogBookEditDTO
        }).pipe(map((logBook: LogBookEditDTO) => {
            for (const firstSalePage of logBook.firstSalePages ?? []) {
                firstSalePage.statusName = this.catchesAndSalesCommonService.getPageStatusTranslation(firstSalePage.status!);
            }

            for (const admissionPage of logBook.admissionPagesAndDeclarations ?? []) {
                admissionPage.statusName = this.catchesAndSalesCommonService.getPageStatusTranslation(admissionPage.status!);
            }

            for (const transportationPage of logBook.transportationPagesAndDeclarations ?? []) {
                transportationPage.statusName = this.catchesAndSalesCommonService.getPageStatusTranslation(transportationPage.status!);
            }

            return logBook;
        }));;
    }

    public getPermitLicenseLogBook(logBookPermitLicenseId: number): Observable<CommercialFishingLogBookEditDTO> {
        throw new Error('Should call getLogBook method instead');
    }

    public getLogBookPagesAndDeclarations(logBookId: number, permitLicenseId: number, logBookType: LogBookTypesEnum): Observable<ShipLogBookPageRegisterDTO[] | AdmissionLogBookPageRegisterDTO[] | TransportationLogBookPageRegisterDTO[]> {
        throw new Error('Should call getLogBookPages method instead');
    }

    public getLogBookPages(logBookId: number, logBookType: LogBookTypesEnum): Observable<FirstSaleLogBookPageRegisterDTO[] | AdmissionLogBookPageRegisterDTO[] | TransportationLogBookPageRegisterDTO[]> {
        const params: HttpParams = new HttpParams().append('logBookId', logBookId.toString());

        let responseTypeCtr: (new (...args: any[]) => any) | undefined = undefined;
        let requestMethod: string | undefined = undefined;

        switch (logBookType) {
            case LogBookTypesEnum.FirstSale:
                responseTypeCtr = FirstSaleLogBookPageRegisterDTO;
                requestMethod = 'GetFirstSaleLogBookPages';
                break;
            case LogBookTypesEnum.Admission:
                responseTypeCtr = AdmissionLogBookPageRegisterDTO;
                requestMethod = 'GetAdmissionLogBookPages';
                break;
            case LogBookTypesEnum.Transportation:
                responseTypeCtr = TransportationLogBookPageRegisterDTO;
                requestMethod = 'GetTransportationLogBookPages';
                break;
            default:
                throw new Error(`invalid log book type (${LogBookTypesEnum[logBookType]}) of getLogBookPages of buyer log book.`);
                break;
        }

        return this.requestService.get<
            FirstSaleLogBookPageRegisterDTO[]
            | AdmissionLogBookPageRegisterDTO[]
            | TransportationLogBookPageRegisterDTO[]>(this.area, this.controller, requestMethod, {
                httpParams: params,
                responseTypeCtr: responseTypeCtr
            }).pipe(map((entries: FirstSaleLogBookPageRegisterDTO[] | AdmissionLogBookPageRegisterDTO[] | TransportationLogBookPageRegisterDTO[]) => {
                if (entries[0] instanceof FirstSaleLogBookPageRegisterDTO) {
                    for (const firstSalePage of entries) {
                        firstSalePage.statusName = this.catchesAndSalesCommonService.getPageStatusTranslation(firstSalePage.status!);
                    }
                }
                else if (entries[0] instanceof AdmissionLogBookPageRegisterDTO) {
                    for (const admissionPage of entries) {
                        admissionPage.statusName = this.catchesAndSalesCommonService.getPageStatusTranslation(admissionPage.status!);
                    }
                }
                else if (entries[0] instanceof TransportationLogBookPageRegisterDTO) {
                    for (const transportationPage of entries) {
                        transportationPage.statusName = this.catchesAndSalesCommonService.getPageStatusTranslation(transportationPage.status!);
                    }
                }

                return entries;
            }));
    }

    public addLogBook(model: LogBookEditDTO, registerId: number, ignoreLogBookConflicts: boolean): Observable<void> {
        const params: HttpParams = new HttpParams()
            .append('registerId', registerId.toString())
            .append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString());

        return this.requestService.post(this.area, this.controller, 'AddLogBook', model, {
            httpParams: params
        });
    }

    public editLogBook(model: LogBookEditDTO, registerId: number, ignoreLogBookConflicts: boolean): Observable<void> {
        const params: HttpParams = new HttpParams()
            .append('registerId', registerId.toString())
            .append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString());

        return this.requestService.put(this.area, this.controller, 'EditLogBook', model, {
            httpParams: params
        });
    }

    public addLogBooksFromOldPermitLicenses(logBooks: CommercialFishingLogBookEditDTO[], permitLicenseId: number, ignoreLogBookConflicts: boolean): Observable<void> {
        throw new Error('Not implemented funcionality for buyer log book pages');
    }

    public getOverlappedLogBooks(parameters: OverlappingLogBooksParameters[]): Observable<RangeOverlappingLogBooksDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetOverlappedLogBooks', parameters, {
            responseTypeCtr: RangeOverlappingLogBooksDTO
        });
    }

    public getLogBooksForRenewal(permitLicenseRegisterId: number, showFinished: boolean): Observable<LogBookForRenewalDTO[]> {
        throw new Error('Functionality not implemented so far.');
    }

    public getLogBooksForRenewalByIds(permitLicenseRegisterIds: number[]): Observable<LogBookEditDTO[]> {
        throw new Error('Functionality not implemented so far.');
    }

    public deleteLogBook(logBookId: number): Observable<void> {
        const params: HttpParams = new HttpParams().append('id', logBookId.toString());

        return this.requestService.delete(this.area, this.controller, 'DeleteLogBook', {
            httpParams: params
        });
    }

    public deleteLogBookPermitLicense(id: number): Observable<void> {
        throw new Error('Should call deleteLogBook method instead.');
    }

    public undoDeleteLogBook(logBookId: number): Observable<void> {
        const params: HttpParams = new HttpParams().append('id', logBookId.toString());

        return this.requestService.patch(this.area, this.controller, 'UndoDeleteLogBook', undefined, {
            httpParams: params
        });
    }

    public undoDeleteLogBookPermitLicense(id: number): Observable<void> {
        throw new Error('Should call undoDeleteLogBook method instead.');
    }

    // audits

    public getPremiseUsageDocumentAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.get(this.area, this.controller, 'GetPremiseUsageDocumentAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getLogBookAudit(id: number): Observable<SimpleAuditDTO> {
        const params: HttpParams = new HttpParams().append('id', id.toString());
        return this.requestService.get(this.area, this.controller, 'GetLogBookAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getBuyerLicensesAudit(id: number): Observable<SimpleAuditDTO> {
        const params: HttpParams = new HttpParams().append('id', id.toString());
        return this.requestService.get(this.area, this.controller, 'GetBuyerLicenseAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    // applications
    public getApplication(id: number, getRegiXData: boolean, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams()
            .append('id', id.toString())
            .append('getRegiXData', getRegiXData.toString());

        switch (pageCode) {
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter: {
                return this.requestService.get(this.area, this.controller, 'GetApplication', {
                    httpParams: params,
                    responseTypeCtr: BuyerApplicationEditDTO
                });
            }
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter: {
                return this.requestService.get(this.area, this.controller, 'GetBuyerChangeOfCircumstancesApplication', {
                    httpParams: params,
                    responseTypeCtr: BuyerChangeOfCircumstancesApplicationDTO
                });
            }
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter: {
                return this.requestService.get(this.area, this.controller, 'GetBuyerTerminationApplication', {
                    httpParams: params,
                    responseTypeCtr: BuyerTerminationApplicationDTO
                });
            }
            default: {
                throw new Error('Invalid page code for getApplication: ' + PageCodeEnum[pageCode]);
            }
        }
    }

    public getRegixData(applicationId: number, pageCode: PageCodeEnum): Observable<RegixChecksWrapperDTO<IApplicationRegister>> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        switch (pageCode) {
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter: {
                return this.requestService.get<RegixChecksWrapperDTO<BuyerRegixDataDTO>>(this.area, this.controller, 'GetRegixData', {
                    httpParams: params,
                    responseTypeCtr: RegixChecksWrapperDTO
                }).pipe(map((result: RegixChecksWrapperDTO<BuyerRegixDataDTO>) => {
                    result.dialogDataModel = new BuyerRegixDataDTO(result.dialogDataModel);
                    result.regiXDataModel = new BuyerRegixDataDTO(result.regiXDataModel);

                    return result;
                }));
            }
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter: {
                return this.requestService.get<RegixChecksWrapperDTO<BuyerChangeOfCircumstancesRegixDataDTO>>(this.area, this.controller, 'GetBuyerChangeOfCircumstancesRegixData', {
                    httpParams: params,
                    responseTypeCtr: RegixChecksWrapperDTO
                }).pipe(map((result: RegixChecksWrapperDTO<BuyerChangeOfCircumstancesRegixDataDTO>) => {
                    result.dialogDataModel = new BuyerChangeOfCircumstancesRegixDataDTO(result.dialogDataModel);
                    result.regiXDataModel = new BuyerChangeOfCircumstancesRegixDataDTO(result.regiXDataModel);

                    return result;
                }));
            }
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter: {
                return this.requestService.get<RegixChecksWrapperDTO<BuyerTerminationBaseRegixDataDTO>>(this.area, this.controller, 'GetBuyerTerminationRegixData', {
                    httpParams: params,
                    responseTypeCtr: RegixChecksWrapperDTO
                }).pipe(map((result: RegixChecksWrapperDTO<BuyerTerminationBaseRegixDataDTO>) => {
                    result.dialogDataModel = new BuyerTerminationBaseRegixDataDTO(result.dialogDataModel);
                    result.regiXDataModel = new BuyerTerminationBaseRegixDataDTO(result.regiXDataModel);

                    return result;
                }));
            }
            default: {
                throw new Error('Invalid page code for getRegixData: ' + PageCodeEnum[pageCode]);
            }
        }
    }

    public getApplicationDataForRegister(applicationId: number): Observable<BuyerEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetApplicationDataForRegister', {
            httpParams: params,
            responseTypeCtr: BuyerEditDTO
        });
    }

    public addApplication(application: IApplicationRegister, pageCode: PageCodeEnum): Observable<number> {
        let method: string = '';

        switch (pageCode) {
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter: {
                method = 'AddApplication';
            } break;
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter: {
                method = 'AddBuyerChangeOfCircumstancesApplication';
            } break;
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter: {
                method = 'AddBuyerTerminationApplication'
            } break;
        }

        return this.requestService.post(this.area, this.controller, method, application, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editApplication(application: IApplicationRegister, pageCode: PageCodeEnum, fromSaveAsDraft: boolean = false): Observable<number> {
        let method: string = '';

        switch (pageCode) {
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter: {
                method = 'EditApplication';
            } break;
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter: {
                method = 'EditBuyerChangeOfCircumstancesApplication';
            } break;
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter: {
                method = 'EditBuyerTerminationApplication';
            } break;
        }

        const params: HttpParams = new HttpParams().append('saveAsDraft', fromSaveAsDraft.toString());

        return this.requestService.post(this.area, this.controller, method, application, {
            httpParams: params,
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editApplicationDataAndStartRegixChecks(model: IApplicationRegister): Observable<void> {
        let method: string = '';

        switch (model.pageCode) {
            case PageCodeEnum.RegFirstSaleBuyer:
            case PageCodeEnum.RegFirstSaleCenter: {
                method = 'EditApplicationAndStartRegixChecks';
            } break;
            case PageCodeEnum.ChangeFirstSaleBuyer:
            case PageCodeEnum.ChangeFirstSaleCenter: {
                method = 'EditBuyerChangeOfCircumstancesApplicationAndStartRegixChecks';
            } break;
            case PageCodeEnum.TermFirstSaleBuyer:
            case PageCodeEnum.TermFirstSaleCenter: {
                method = 'EditBuyerTerminationApplicationAndStartRegiXChecks';
            } break;
        }

        return this.requestService.post(this.area, this.controller, method, model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    // change of circumstances
    public getBuyerFromChangeOfCircumstancesApplication(applicationId: number): Observable<BuyerEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetBuyerFromChangeOfCircumstancesApplication', {
            httpParams: params,
            responseTypeCtr: BuyerEditDTO
        });
    }

    public completeBuyerChangeOfCircumstancesApplication(buyer: BuyerEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'CompleteBuyerChangeOfCircumstancesApplication', buyer, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public getBuyerFromTerminationApplication(applicationId: number): Observable<BuyerEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetBuyerFromTerminationApplication', {
            httpParams: params,
            responseTypeCtr: BuyerEditDTO
        });
    }

    // nomenclatures
    public getBuyerTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetBuyerTypes', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getAllBuyersNomenclatures(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllBuyersNomenclatures', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getAllFirstSaleCentersNomenclatures(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllFirstSaleCentersNomenclatures', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getBuyerStatuses(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetBuyerStatuses', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public updateBuyerStatus(buyerId: number, status: CancellationHistoryEntryDTO, applicationId?: number): Observable<void> {
        let params = new HttpParams().append('buyerId', buyerId.toString());

        if (applicationId !== undefined && applicationId !== null) {
            params = params.append('applicationId', applicationId.toString());
        }

        return this.requestService.put(this.area, this.controller, 'UpdateBuyerStatus', status, {
            httpParams: params
        });
    }

    // helpers

    private getLogBooksForTableHelper(controller: string, buyerIds: number[], filters: BuyersFilters | undefined): Observable<LogBookDetailDTO[]> {
        return this.requestService.post(this.area, controller, 'GetLogBooksForTable', buyerIds, {
            responseTypeCtr: LogBookDetailDTO
        });
    }
}