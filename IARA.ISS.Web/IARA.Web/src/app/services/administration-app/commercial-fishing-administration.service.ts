﻿import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { CommercialFishingApplicationEditDTO } from '@app/models/generated/dtos/CommercialFishingApplicationEditDTO';
import { CommercialFishingEditDTO } from '@app/models/generated/dtos/CommercialFishingEditDTO';
import { CommercialFishingPermitRegisterDTO } from '@app/models/generated/dtos/CommercialFishingPermitRegisterDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { CommercialFishingRegisterFilters } from '@app/models/generated/filters/CommercialFishingRegisterFilters';
import { RequestService } from '@app/shared/services/request.service';
import { ApplicationsProcessingService } from './applications-processing.service';
import { ApplicationsRegisterAdministrativeBaseService } from './applications-register-administrative-base.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RequestProperties } from '@app/shared/services/request-properties';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { CommercialFishingPermitLicenseRegisterDTO } from '@app/models/generated/dtos/CommercialFishingPermitLicenseRegisterDTO';
import { SuspensionTypeNomenclatureDTO } from '@app/models/generated/dtos/SuspensionTypeNomenclatureDTO';
import { SuspensionReasonNomenclatureDTO } from '@app/models/generated/dtos/SuspensionReasonNomenclatureDTO';
import { PermitNomenclatureDTO } from '@app/models/generated/dtos/PermitNomenclatureDTO';
import { PermitLicenseForRenewalDTO } from '@app/models/generated/dtos/PermitLicenseForRenewalDTO';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { PoundNetNomenclatureDTO } from '@app/models/generated/dtos/PoundNetNomenclatureDTO';
import { QualifiedFisherNomenclatureDTO } from '@app/models/generated/dtos/QualifiedFisherNomenclatureDTO';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionsEnum } from '@app/shared/enums/permissions.enum';
import { PaymentTariffDTO } from '@app/models/generated/dtos/PaymentTariffDTO';
import { PermitLicenseTariffCalculationParameters } from '@app/components/common-app/commercial-fishing/models/permit-license-tariff-calculation-parameters.model';
import { OverlappingLogBooksParameters } from '@app/shared/components/overlapping-log-books/models/overlapping-log-books-parameters.model';
import { RangeOverlappingLogBooksDTO } from '@app/models/generated/dtos/RangeOverlappingLogBooksDTO';
import { CatchesAndSalesCommonService } from '@app/services/common-app/catches-and-sales-common.service';
import { ILogBookService } from '@app/components/common-app/commercial-fishing/components/edit-log-book/interfaces/log-book.interface';
import { CommercialFishingLogBookEditDTO } from '@app/models/generated/dtos/CommercialFishingLogBookEditDTO';
import { LogBookForRenewalDTO } from '@app/models/generated/dtos/LogBookForRenewalDTO';
import { RegisterDTO } from '@app/models/generated/dtos/RegisterDTO';
import { LogBookEditDTO } from '@app/models/generated/dtos/LogBookEditDTO';
import { FirstSaleLogBookPageRegisterDTO } from '@app/models/generated/dtos/FirstSaleLogBookPageRegisterDTO';
import { AdmissionLogBookPageRegisterDTO } from '@app/models/generated/dtos/AdmissionLogBookPageRegisterDTO';
import { TransportationLogBookPageRegisterDTO } from '@app/models/generated/dtos/TransportationLogBookPageRegisterDTO';
import { AquacultureLogBookPageRegisterDTO } from '@app/models/generated/dtos/AquacultureLogBookPageRegisterDTO';
import { ShipLogBookPageRegisterDTO } from '@app/models/generated/dtos/ShipLogBookPageRegisterDTO';
import { LogBookTypesEnum } from '@app/enums/log-book-types.enum';
import { SuspensionDataDTO } from '@app/models/generated/dtos/SuspensionDataDTO';
import { ISuspensionService } from '@app/interfaces/common-app/suspension.interface';
import { InspectedPermitLicenseNomenclatureDTO } from '@app/models/generated/dtos/InspectedPermitLicenseNomenclatureDTO';
import { FishingGearForChoiceDTO } from '@app/models/generated/dtos/FishingGearForChoiceDTO';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { PermitLicenseFishingGearsApplicationDTO } from '@app/models/generated/dtos/PermitLicenseFishingGearsApplicationDTO';
import { PermitLicensesNomenclatureDTO } from '@app/models/generated/dtos/PermitLicensesNomenclatureDTO';
import { IdentifierTypeEnum } from '@app/enums/identifier-type.enum';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';

@Injectable({
    providedIn: 'root'
})
export class CommercialFishingAdministrationService extends ApplicationsRegisterAdministrativeBaseService implements ICommercialFishingService, ILogBookService, ISuspensionService {
    protected controller: string = 'CommercialFishingAdministration';

    private readonly translate: FuseTranslationLoaderService;
    private readonly permissions: PermissionsService;
    private readonly catchesAndSalesCommonService: CatchesAndSalesCommonService;

    public constructor(requestService: RequestService,
        applicationsProcessingService: ApplicationsProcessingService,
        translate: FuseTranslationLoaderService,
        permissions: PermissionsService,
        catchesAndSalesCommonService: CatchesAndSalesCommonService
    ) {
        super(requestService, applicationsProcessingService);
        this.translate = translate;
        this.permissions = permissions;
        this.catchesAndSalesCommonService = catchesAndSalesCommonService;
    }

    // Register
    public getAllPermits(request: GridRequestModel<CommercialFishingRegisterFilters>): Observable<GridResultModel<CommercialFishingPermitRegisterDTO>> {
        type Result = GridResultModel<CommercialFishingPermitRegisterDTO>;
        type Body = GridRequestModel<CommercialFishingRegisterFilters>;

        return this.requestService.post<Result, Body>(this.area, this.controller, 'GetAllPermits', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        }).pipe(switchMap((entries: Result) => {
            const permitIds: number[] = entries.records.map((permit: CommercialFishingPermitRegisterDTO) => {
                return permit.id!;
            });

            if (permitIds.length === 0) {
                return of(entries);
            }

            if (this.permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterReadAll)
                || this.permissions.has(PermissionsEnum.CommercialFishingPermitLicenseRegisterRead)) {
                return this.getPermitLicensesForTable(this.controller, permitIds, request.filters).pipe(map((permitLicenses: CommercialFishingPermitLicenseRegisterDTO[]) => {
                    for (const permitLicense of permitLicenses) {
                        const found = entries.records.find((entry: CommercialFishingPermitLicenseRegisterDTO) => {
                            return entry.id === permitLicense.permitId;
                        });

                        if (found !== undefined) {
                            if (found.licenses !== undefined && found.licenses !== null) {
                                found.licenses.push(new CommercialFishingPermitLicenseRegisterDTO(permitLicense));
                            }
                            else {
                                found.licenses = [permitLicense];
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

    public getRecord(id: number, pageCode: PageCodeEnum): Observable<CommercialFishingEditDTO> {
        const params = new HttpParams().append('id', id.toString());
        let serviceMethod: string = '';

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                serviceMethod = 'GetPermit';
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                serviceMethod = 'GetPermitLicense';
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        return this.requestService.get<CommercialFishingEditDTO>(this.area, this.controller, serviceMethod, {
            httpParams: params,
            responseTypeCtr: CommercialFishingEditDTO
        });
    }

    public getOverlappedLogBooks(parameters: OverlappingLogBooksParameters[]): Observable<RangeOverlappingLogBooksDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetOverlappedLogBooks', parameters, {
            responseTypeCtr: RangeOverlappingLogBooksDTO
        });
    }

    public getLogBooksForRenewal(permitLicenseRegisterId: number, showFinished: boolean, showOnlySameAquaticTypes: boolean): Observable<LogBookForRenewalDTO[]> {
        const params: HttpParams = new HttpParams()
            .append('permitLicenseRegisterId', permitLicenseRegisterId.toString())
            .append('showFinished', showFinished.toString())
            .append('showOnlySameAquaticTypes', showOnlySameAquaticTypes.toString());

        return this.requestService.get(this.area, this.controller, 'GetLogBooksForRenewal', {
            httpParams: params,
            responseTypeCtr: LogBookForRenewalDTO
        });
    }

    public getLogBooksForRenewalByIds(permitLicenseRegisterIds: number[]): Observable<CommercialFishingLogBookEditDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetLogBooksForRenewalByIds', permitLicenseRegisterIds, {
            responseTypeCtr: CommercialFishingLogBookEditDTO
        });
    }

    public getRegisterByApplicationId(applicationId: number, pageCode?: PageCodeEnum): Observable<unknown> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        let serviceMethod: string = '';

        switch (pageCode!) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                serviceMethod = 'GetPermitRegisterByApplicationId';
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                serviceMethod = 'GetPermitLicenseRegisterByApplicationId';
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode!]}`);
        }

        return this.requestService.get(this.area, this.controller, serviceMethod, {
            httpParams: params,
            responseTypeCtr: CommercialFishingEditDTO
        });
    }

    public addPermit(permit: CommercialFishingEditDTO, pageCode: PageCodeEnum, ignoreLogBookConflicts: boolean): Observable<number> {
        let serviceMethod: string = '';
        let params: HttpParams | undefined;

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                {
                    serviceMethod = 'AddPermit';
                }
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                {
                    serviceMethod = 'AddPermitLicense';
                    params = new HttpParams().append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString());
                }
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        return this.requestService.post(this.area, this.controller, serviceMethod, permit, {
            properties: new RequestProperties({ asFormData: true }),
            httpParams: params
        });
    }

    public addAndDownloadRegister(
        model: CommercialFishingEditDTO,
        pageCode: PageCodeEnum,
        ignoreLogBookConflicts: boolean
    ): Observable<boolean> {
        let serviceMethod: string = '';
        let params: HttpParams | undefined;

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                {
                    serviceMethod = 'AddPermitAndDownloadRegister';
                }
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                {
                    serviceMethod = 'AddPermitLicenseAndDownloadRegister';
                    params = new HttpParams().append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString());
                }
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        const registerDto: RegisterDTO<CommercialFishingEditDTO> = new RegisterDTO<CommercialFishingEditDTO>({
            dto: model
        });

        return this.requestService.downloadPost(this.area, this.controller, serviceMethod, '', registerDto, {
            properties: new RequestProperties({ asFormData: true }),
            httpParams: params
        });
    }

    public downloadRegister(id: number, pageCode: PageCodeEnum): Observable<boolean> {
        let methodName: string = '';

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                methodName = 'DownloadPermitRegister';
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                methodName = 'DownloadPermitLicenseRegister';
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        const params = new HttpParams().append('registerId', id.toString());
        return this.requestService.downloadPost(this.area, this.controller, methodName, '', undefined, { httpParams: params });
    }

    public editPermit(permit: CommercialFishingEditDTO, pageCode: PageCodeEnum, ignoreLogBookConflicts: boolean): Observable<void> {
        let serviceMethod: string = '';
        let params: HttpParams | undefined;

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                {
                    serviceMethod = 'EditPermit';
                }
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                {
                    serviceMethod = 'EditPermitLicense';
                    params = new HttpParams().append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString());
                }
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        return this.requestService.post(this.area, this.controller, serviceMethod, permit, {
            properties: new RequestProperties({ asFormData: true }),
            httpParams: params
        });
    }

    public editAndDownloadRegister(
        model: CommercialFishingEditDTO,
        pageCode: PageCodeEnum,
        ignoreLogBookConflicts: boolean
    ): Observable<boolean> {
        let serviceMethod: string = '';
        let params: HttpParams | undefined;

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                {
                    serviceMethod = 'EditPermitAndDownloadRegister';
                }
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                {
                    serviceMethod = 'EditPermitLicenseAndDownloadRegister';
                    params = new HttpParams().append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString());
                }
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        const register: RegisterDTO<CommercialFishingEditDTO> = new RegisterDTO<CommercialFishingEditDTO>({
            dto: model
        });

        return this.requestService.downloadPost(this.area, this.controller, serviceMethod, '', register, {
            properties: new RequestProperties({ asFormData: true }),
            httpParams: params
        });
    }

    public getPermitLicenseFisherPhoto(id: number): Observable<string> {
        const params = new HttpParams().append('permitLicenseId', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPermitLicenseFisherPhoto', {
            httpParams: params,
            responseType: 'text',
            properties: RequestProperties.NO_SPINNER
        });
    }

    public getPermitLicenseFisherPhotoFromApplication(applicationId: number): Observable<string> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetPermitLicenseFisherPhotoFromApplication', {
            httpParams: params,
            responseType: 'text',
            properties: RequestProperties.NO_SPINNER
        });
    }

    public getPermitFisherPhoto(id: number): Observable<string> {
        const params = new HttpParams().append('permitId', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPermitFisherPhoto', {
            httpParams: params,
            responseType: 'text',
            properties: RequestProperties.NO_SPINNER
        });
    }

    public getPermitFisherPhotoFromApplication(applicationId: number): Observable<string> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetPermitFisherPhotoFromApplication', {
            httpParams: params,
            responseType: 'text',
            properties: RequestProperties.NO_SPINNER
        });
    }

    public getSuspensions(recordId: number, pageCode: PageCodeEnum): Observable<SuspensionDataDTO[]> {
        let serviceMethod: string = '';

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry: {
                serviceMethod = 'GetPermitSuspensions';
            } break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies: {
                serviceMethod = 'GetPermitLicenseSuspensions';
            } break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        const params: HttpParams = new HttpParams().append('id', recordId.toString());

        return this.requestService.get(this.area, this.controller, serviceMethod, {
            httpParams: params,
            responseTypeCtr: SuspensionDataDTO
        });
    }

    public addSuspension(suspension: SuspensionDataDTO, id: number, pageCode: PageCodeEnum): Observable<void> {
        let params: HttpParams = new HttpParams();
        let serviceMethod: string = '';

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry: {
                serviceMethod = 'AddPermitSuspension';
                params = params.append('permitId', id.toString());
            } break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies: {
                serviceMethod = 'AddPermitLicenseSuspension';
                params = params.append('permitLicenseId', id.toString());
            } break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        return this.requestService.post(this.area, this.controller, serviceMethod, suspension, {
            httpParams: params
        });
    }

    public addEditSuspensions(recordId: number, suspensions: SuspensionDataDTO[], pageCode: PageCodeEnum): Observable<void> {
        let serviceMethod: string = '';
        let successMessage: string = '';
        let params: HttpParams = new HttpParams();

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry: {
                serviceMethod = 'AddEditPermitSuspensions';
                successMessage = 'succ-change-permit-suspensions';
                params = params.append('permitId', recordId.toString());
            } break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies: {
                serviceMethod = 'AddEditPermitLicenseSuspensions';
                successMessage = 'succ-change-permit-license-suspensions';
                params = params.append('permitLicenseId', recordId.toString());
            } break;
            default:
                throw new Error(`Invalid pageCode for commercial fishing add or edit suspensions: ${PageCodeEnum[pageCode]}`);
        }

        return this.requestService.post(this.area, this.controller, serviceMethod, suspensions, {
            httpParams: params,
            successMessage: successMessage
        });
    }

    public deletePermit(id: number, pageCode: PageCodeEnum): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        let serviceMethod: string = '';

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                serviceMethod = 'DeletePermit';
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                serviceMethod = 'DeletePermitLicense';
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        return this.requestService.delete(this.area, this.controller, serviceMethod, { httpParams: params });
    }

    public undoDeletePermit(id: number, pageCode: PageCodeEnum): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        let serviceMethod: string = '';

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                serviceMethod = 'UndoDeletePermit';
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                serviceMethod = 'UndoDeletePermitLicense';
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        return this.requestService.patch(this.area, this.controller, serviceMethod, null, { httpParams: params });
    }

    // log books

    public getLogBooksForTable(permitLicenseId: number): Observable<CommercialFishingLogBookEditDTO[]> {
        const params: HttpParams = new HttpParams().append('permitLicenseId', permitLicenseId.toString());

        return this.requestService.get(this.area, this.controller, 'GetLogBooksForTable', {
            httpParams: params,
            responseTypeCtr: CommercialFishingLogBookEditDTO
        });
    }

    public getPermitLicenseLogBook(logBookPermitLicenseId: number): Observable<CommercialFishingLogBookEditDTO> {
        const params: HttpParams = new HttpParams().append('logBookPermitLicenseId', logBookPermitLicenseId.toString());

        return this.requestService.get(this.area, this.controller, 'GetPermitLicenseLogBook', {
            responseTypeCtr: CommercialFishingLogBookEditDTO,
            httpParams: params
        });
    }

    public getLogBook(logBookId: number): Observable<LogBookEditDTO> {
        throw new Error('Method getPermitLicenseLogBook should be called instead.');
    }

    public getLogBookPagesAndDeclarations(logBookId: number, permitLicenseId: number | undefined, logBookType: LogBookTypesEnum): Observable<ShipLogBookPageRegisterDTO[] | AdmissionLogBookPageRegisterDTO[] | TransportationLogBookPageRegisterDTO[]> {
        let params: HttpParams = new HttpParams()
            .append('logBookId', logBookId.toString());

        if (permitLicenseId !== null && permitLicenseId !== undefined) {
            params = params.append('permitLicenseId', permitLicenseId.toString());
        }

        let responseTypeCtr: (new (...args: any[]) => any) | undefined = undefined;
        let requestMethod: string | undefined = undefined;

        switch (logBookType) {
            case LogBookTypesEnum.Ship:
                responseTypeCtr = ShipLogBookPageRegisterDTO;
                requestMethod = 'GetShipLogBookPagesAndDeclarations';
                break;
            case LogBookTypesEnum.Admission:
                responseTypeCtr = AdmissionLogBookPageRegisterDTO;
                requestMethod = 'GetShipAdmissionLogBookPagesAndDeclarations';
                break;
            case LogBookTypesEnum.Transportation:
                responseTypeCtr = TransportationLogBookPageRegisterDTO;
                requestMethod = 'GetTransportationLogBookPagesAndDeclarations';
                break;
            default:
                throw new Error(`Invalid log book type (${LogBookTypesEnum[logBookType]}) of getLogBookPages of buyer log book.`);
                break;
        }

        return this.requestService.get<
            ShipLogBookPageRegisterDTO[]
            | AdmissionLogBookPageRegisterDTO[]
            | TransportationLogBookPageRegisterDTO[]>(this.area, this.controller, requestMethod, {
                httpParams: params,
                responseTypeCtr: responseTypeCtr
            }).pipe(map((entries: ShipLogBookPageRegisterDTO[] | AdmissionLogBookPageRegisterDTO[] | TransportationLogBookPageRegisterDTO[]) => {
                if (entries[0] instanceof ShipLogBookPageRegisterDTO) {
                    for (const shipPage of entries) {
                        shipPage.statusName = this.catchesAndSalesCommonService.getPageStatusTranslation(shipPage.status!);
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

    public getLogBookPages(logBookId: number, logBookType: LogBookTypesEnum): Observable<FirstSaleLogBookPageRegisterDTO[]
        | AdmissionLogBookPageRegisterDTO[]
        | TransportationLogBookPageRegisterDTO[]
        | AquacultureLogBookPageRegisterDTO[]
    > {
        throw new Error('Method getLogBookPagesAndDeclarations should be called instead.');
    }

    public addLogBook(model: CommercialFishingLogBookEditDTO, registerId: number, ignoreLogBookConflicts: boolean): Observable<void> {
        const params: HttpParams = new HttpParams()
            .append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString())
            .append('registerId', registerId.toString());

        return this.requestService.post(this.area, this.controller, 'AddLogBook', model, {
            httpParams: params
        });
    }

    public editLogBook(model: CommercialFishingLogBookEditDTO, registerId: number, ignoreLogBookConflicts: boolean): Observable<void> {
        const params: HttpParams = new HttpParams()
            .append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString())
            .append('registerId', registerId.toString());

        return this.requestService.post(this.area, this.controller, 'EditLogBook', model, {
            httpParams: params
        });
    }

    public addLogBooksFromOldPermitLicenses(logBooks: CommercialFishingLogBookEditDTO[], permitLicenseId: number, ignoreLogBookConflicts: boolean): Observable<void> {
        const params: HttpParams = new HttpParams()
            .append('permitLicenseId', permitLicenseId.toString())
            .append('ignoreLogBookConflicts', ignoreLogBookConflicts.toString());

        return this.requestService.post(this.area, this.controller, 'AddLogBookFromOldPermitLicenses', logBooks, {
            httpParams: params
        });
    }

    public deleteLogBookPermitLicense(logBookPermitLicenseId: number): Observable<void> {
        const params: HttpParams = new HttpParams().append('id', logBookPermitLicenseId.toString());

        return this.requestService.delete(this.area, this.controller, 'DeleteLogBookPermitLicense', {
            httpParams: params
        });
    }

    public undoDeleteLogBookPermitLicense(logBookPermitLicenseId: number): Observable<void> {
        const params: HttpParams = new HttpParams().append('id', logBookPermitLicenseId.toString());

        return this.requestService.patch(this.area, this.controller, 'UndoDeleteLogBookPermitLicense', undefined, {
            httpParams: params
        });
    }

    public deleteLogBook(logBookId: number): Observable<void> {
        throw new Error('Should call deleteLogBookPermitLicense method instead.');
    }

    public undoDeleteLogBook(logBookId: number): Observable<void> {
        throw new Error('Should call undoDeleteLogBookPermitLicense method instead.');
    }

    public getPermitLicenseSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPermitLicenseAuditInfo', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getLogBookAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetLogBookSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getPermitSuspensionAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPermitSuspensionSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getPermitLicenseSuspensionAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPermitLicenseSuspensionSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public downloadPermitFluxXml(permit: CommercialFishingEditDTO): Observable<boolean> {
        permit.files = undefined;
        return this.requestService.downloadPost(this.area, this.controller, 'DownloadPermitFluxXml', '', permit);
    }

    // Fishing gears

    public getFishingGearAudit(id: number): Observable<SimpleAuditDTO> { // TODO maybe add pageCode here and check that method is called only for permit Licenses ???
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetFishingGearSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    // Applications
    public getApplication(id: number, getRegiXData: boolean, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams()
            .append('id', id.toString())
            .append('getRegiXData', getRegiXData.toString());
        let serviceMethod: string = '';

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                serviceMethod = 'GetPermitApplication';
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                serviceMethod = 'GetPermitLicenseApplication';
                break;
            case PageCodeEnum.FishingGearsCommFish:
                serviceMethod = 'GetPermitLicenseFishingGearsApplication';
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        if (pageCode === PageCodeEnum.FishingGearsCommFish) {
            return this.requestService.get(this.area, this.controller, serviceMethod, {
                httpParams: params,
                responseTypeCtr: PermitLicenseFishingGearsApplicationDTO
            });
        }
        else {
            return this.requestService.get(this.area, this.controller, serviceMethod, {
                httpParams: params,
                responseTypeCtr: CommercialFishingApplicationEditDTO
            });
        }
    }

    public getPermitLicensesForRenewal(permitId: number | undefined, permitNumber: string | undefined, pageCode: PageCodeEnum): Observable<PermitLicenseForRenewalDTO[]> {
        const params: HttpParams = new HttpParams().append('permitId', permitId!.toString()).append('pageCode', pageCode.toString());
        return this.requestService.get(this.area, this.controller, 'GetPermitLicensesForRenewal', {
            httpParams: params,
            responseTypeCtr: PermitLicenseForRenewalDTO
        });
    }

    public getPermitLicenseData(permitLicenseId: number): Observable<CommercialFishingApplicationEditDTO> {
        const params: HttpParams = new HttpParams().append('permitLicenseId', permitLicenseId.toString());
        return this.requestService.get(this.area, this.controller, 'GetPermitLicenseData', {
            httpParams: params,
            responseTypeCtr: CommercialFishingApplicationEditDTO
        });
    }

    public getPermitLicenseApplicationDataFromPermitId(permitId: number, applicationId: number): Observable<CommercialFishingApplicationEditDTO> {
        const params: HttpParams = new HttpParams().append('permitId', permitId.toString()).append('applicationId', applicationId.toString());;
        return this.requestService.get(this.area, this.controller, 'GetPermitLicenseApplicationDataFromPermit', {
            httpParams: params,
            responseTypeCtr: CommercialFishingApplicationEditDTO
        });
    };

    public getPermitLicenseApplicationDataFromPermitNumber(permitNumber: string, applicationId: number): Observable<CommercialFishingApplicationEditDTO> {
        throw new Error('This method should not be called from the administration app.');
    };


    public getFishingGearsByPermitLicenseRegistrationNumber(permitLicenseNumber: string, shipId: number): Observable<FishingGearDTO[]> {
        throw new Error('This method should not be called from the administration app.');
    }

    public getRegixData(id: number, pageCode: PageCodeEnum): Observable<RegixChecksWrapperDTO<IApplicationRegister>> {
        const params = new HttpParams().append('applicationId', id.toString());
        let serviceMethod: string = '';

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                serviceMethod = 'GetPermitRegixData';
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                serviceMethod = 'GetPermitLicenseRegixData';
                break;
            case PageCodeEnum.FishingGearsCommFish:
                serviceMethod = 'GetPermitLicenseFishingGearsRegixData';
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        return this.requestService.get(this.area, this.controller, serviceMethod, {
            httpParams: params,
            responseTypeCtr: RegixChecksWrapperDTO
        });
    }

    public getApplicationDataForRegister(applicationId: number, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        let serviceMethod: string = '';

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                serviceMethod = 'GetPermitApplicationDataForRegister';
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                serviceMethod = 'GetPermitLicenseApplicationDataForRegister';
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        return this.requestService.get(this.area, this.controller, serviceMethod, {
            httpParams: params,
            responseTypeCtr: CommercialFishingEditDTO
        });
    }

    public calculatePermitLicenseAppliedTariffs(tariffCalculationParameters: PermitLicenseTariffCalculationParameters): Observable<PaymentTariffDTO[]> {
        return this.requestService.post(this.area, this.controller, 'CalculatePermitLicenseAppliedTariffs', tariffCalculationParameters, {
            responseTypeCtr: PaymentTariffDTO
        });
    }

    public addApplication(application: IApplicationRegister, pageCode: PageCodeEnum): Observable<number> {
        let serviceMethod: string = '';

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                serviceMethod = 'AddPermitApplication';
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                serviceMethod = 'AddPermitLicenseApplication';
                break;
            case PageCodeEnum.FishingGearsCommFish:
                serviceMethod = 'AddPermitLicenseFishingGearsApplication';
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        return this.requestService.post(this.area, this.controller, serviceMethod, application, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public addPermitApplicationAndStartPermitLicenseApplication(permit: CommercialFishingApplicationEditDTO): Observable<CommercialFishingApplicationEditDTO> {
        return this.requestService.post(this.area, this.controller, 'AddPermitApplicationAndStartPermitLicenseApplication', permit, {
            properties: new RequestProperties({ asFormData: true }),
            responseTypeCtr: CommercialFishingApplicationEditDTO
        });
    }

    public editApplication(application: IApplicationRegister, pageCode: PageCodeEnum, fromSaveAsDraft: boolean): Observable<number> {
        const params = new HttpParams().append('saveAsDraft', fromSaveAsDraft.toString());
        let serviceMethod: string = '';

        switch (pageCode) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                serviceMethod = 'EditPermitApplication';
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                serviceMethod = 'EditPermitLicenseApplication';
                break;
            case PageCodeEnum.FishingGearsCommFish:
                serviceMethod = 'EditPermitLicenseFishingGearsApplication';
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[pageCode]}`);
        }

        return this.requestService.post(this.area, this.controller, serviceMethod, application, {
            httpParams: params,
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editApplicationDataAndStartRegixChecks(model: IApplicationRegister): Observable<void> {
        let serviceMethod: string = '';

        switch (model.pageCode!) {
            case PageCodeEnum.CommFish:
            case PageCodeEnum.PoundnetCommFish:
            case PageCodeEnum.RightToFishThirdCountry:
                serviceMethod = 'EditPermitApplicationAndStartRegixChecks';
                break;
            case PageCodeEnum.RightToFishResource:
            case PageCodeEnum.PoundnetCommFishLic:
            case PageCodeEnum.CatchQuataSpecies:
                serviceMethod = 'EditPermitLicenseApplicationAndStartRegixChecks';
                break;
            case PageCodeEnum.FishingGearsCommFish:
                serviceMethod = 'EditFishingGearApplicationAndStartRegixChecks'; 
                break;
            default:
                throw new Error(`Unknown PageCode for commercial fishing administration service: ${PageCodeEnum[model.pageCode!]}`);
        }

        return this.requestService.post(this.area, this.controller, serviceMethod, model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public getPermitLicenseFromFishingGearsApplication(applicationId: number): Observable<CommercialFishingEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetPermitLicenseFromFishingGearsApplication', {
            httpParams: params,
            responseTypeCtr: CommercialFishingEditDTO
        });
    }

    public completePermitLicenseFishingGearsApplication(permitLicense: CommercialFishingEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'CompletePermitLicenseFishingGearsApplication', permitLicense, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    // Nomenclatures
    public getCommercialFishingPermitTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetCommercialFishingPermitTypes', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getCommercialFishingPermitLicenseTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetCommercialFishingPermitLicenseTypes', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getQualifiedFishers(): Observable<QualifiedFisherNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetQualifiedFishers', {
            responseTypeCtr: QualifiedFisherNomenclatureDTO
        });
    }

    public getWaterTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetWaterTypes', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getFishingGearMarkStatuses(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetFishingGearMarkStatuses', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getFishingGearPingerStatuses(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetFishingGearPingerStatuses', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getHolderGroundForUseTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetHolderGroundForUseTypes', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getPoundNets(): Observable<PoundNetNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetPoundNets', {
            responseTypeCtr: PoundNetNomenclatureDTO
        });
    }

    public getSuspensionTypes(): Observable<SuspensionTypeNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetSuspensionTypes', { responseTypeCtr: SuspensionTypeNomenclatureDTO });
    }

    public getSuspensionReasons(): Observable<SuspensionReasonNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetSuspensionReasons', { responseTypeCtr: SuspensionReasonNomenclatureDTO });
    }

    public getPermitNomenclatures(shipId: number, showPastPermits: boolean, onlyPoundNet: boolean): Observable<PermitNomenclatureDTO[]> {
        const params = new HttpParams()
            .append('shipId', shipId.toString())
            .append('showPastPermits', showPastPermits.toString())
            .append('onlyPoundNet', onlyPoundNet.toString());

        return this.requestService.get<PermitNomenclatureDTO[]>(this.area, this.controller, 'GetShipPermits', {
            httpParams: params,
            responseTypeCtr: PermitNomenclatureDTO
        }).pipe(map((permits: PermitNomenclatureDTO[]) => {
            for (const permit of permits) {
                const shipOwnerNamesResource: string = this.translate.getValue('commercial-fishing.permit-nomenclature-ship-owner-names');
                const captain: string = this.translate.getValue('commercial-fishing.permit-nomenclature-captain');

                if (permit.displayName !== null && permit.displayName !== undefined && permit.displayName!.length > 0) {
                    permit.displayName += ` | ${permit.waterTypeName} | ${shipOwnerNamesResource}: ${permit.shipOwnerName} | ${captain}: ${permit.captainName}`;
                }
                else {
                    permit.displayName = `${permit.waterTypeName} | ${shipOwnerNamesResource}: ${permit.shipOwnerName} | ${captain}: ${permit.captainName}`;
                }
            }

            return permits;
        }));
    }

    public getPermitLicensesNomenclatures(shipId: number): Observable<PermitLicensesNomenclatureDTO[]> {
        const params = new HttpParams().append('shipId', shipId.toString());

        return this.requestService.get<PermitLicensesNomenclatureDTO[]>(this.area, this.controller, 'GetShipPermitLicenses', {
            httpParams: params,
            responseTypeCtr: PermitNomenclatureDTO
        }).pipe(map((permits: PermitNomenclatureDTO[]) => {
            for (const permit of permits) {
                const captain: string = this.translate.getValue('commercial-fishing.permit-nomenclature-captain');

                if (permit.displayName !== null && permit.displayName !== undefined && permit.displayName!.length > 0) {
                    permit.displayName += ` | ${permit.waterTypeName} | ${captain}: ${permit.captainName}`;
                }
                else {
                    permit.displayName = `${permit.waterTypeName} | ${captain}: ${permit.captainName}`;
                }
            }

            return permits;
        }));
    }

    public getShipPermitLicensesFromInspection(shipId: number): Observable<InspectedPermitLicenseNomenclatureDTO[]> {
        const params: HttpParams = new HttpParams().append('shipId', shipId.toString());

        return this.requestService.get<InspectedPermitLicenseNomenclatureDTO[]>(this.area, this.controller, 'GetShipPermitLicensesFromInspection', {
            httpParams: params,
            responseTypeCtr: InspectedPermitLicenseNomenclatureDTO
        }).pipe(map((licenses: InspectedPermitLicenseNomenclatureDTO[]) => {
            for (const license of licenses) {
                const inspection: string = this.translate.getValue('commercial-fishing.permit-license-nomenclature-inspection-num');
                const year: string = this.translate.getValue('commercial-fishing.permit-license-nomenclature-year');

                if (license.displayName !== null && license.displayName !== undefined && license.displayName.length > 0) {
                    license.displayName += ` | ${year}: ${license.year} | ${inspection}: ${license.inspectionReportNum}`;
                }
                else {
                    license.displayName = `${year}: ${license.year} | ${inspection}: ${license.inspectionReportNum}`;
                }
            }

            return licenses;
        }));
    }

    public getShipFishingGearsFromInspection(inspectionId: number): Observable<FishingGearForChoiceDTO[]> {
        const params: HttpParams = new HttpParams().append('inspectionId', inspectionId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipFishingGearsFromInspection', {
            httpParams: params,
            responseTypeCtr: FishingGearForChoiceDTO
        });
    }

    public getFishingGearsForIds(gearIds: number[]): Observable<FishingGearDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetFishingGearsForIds', gearIds, {
            responseTypeCtr: FishingGearDTO
        });
    }

    public getPermitLicenseFishingGears(permitLicenseId: number): Observable<FishingGearDTO[]> {
        const params: HttpParams = new HttpParams().append('permitLicenseId', permitLicenseId.toString());

        return this.requestService.get(this.area, this.controller, 'GetCommercialFishingPermitLicenseFishingGears', {
            httpParams: params,
            responseTypeCtr: FishingGearDTO
        });
    }

    public tryGetQualifiedFisher(identifierType: IdentifierTypeEnum, identifier: string): Observable<PersonFullDataDTO | undefined> {
        const params = new HttpParams()
            .append('identifierType', identifierType.toString())
            .append('identifier', identifier);

        return this.requestService.get(this.area, this.controller, 'TryGetQualifiedFisher', {
            httpParams: params,
            responseTypeCtr: PersonFullDataDTO
        });
    }

    // helpers

    private getPermitLicensesForTable(controller: string, permitIDs: number[], filters: CommercialFishingRegisterFilters | undefined): Observable<CommercialFishingPermitLicenseRegisterDTO[]> {
        const request = new PermitLicenseData({ filters: filters, permitIds: permitIDs });
        return this.requestService.post(this.area, controller, 'GetPermitLicensesForTable', request, {
            responseTypeCtr: CommercialFishingPermitLicenseRegisterDTO
        });
    }
}

class PermitLicenseData {
    public filters: CommercialFishingRegisterFilters | undefined;
    public permitIds!: number[];

    public constructor(obj?: Partial<PermitLicenseData>) {
        Object.assign(this, obj);
    }
}
