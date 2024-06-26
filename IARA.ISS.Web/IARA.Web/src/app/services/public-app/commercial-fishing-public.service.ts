﻿import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';
import { map } from 'rxjs/operators';

import { ICommercialFishingService } from '@app/interfaces/common-app/commercial-fishing.interface';
import { RequestService } from '@app/shared/services/request.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { CommercialFishingEditDTO } from '@app/models/generated/dtos/CommercialFishingEditDTO';
import { ApplicationsRegisterPublicBaseService } from './applications-register-public-base.service';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { CommercialFishingApplicationEditDTO } from '@app/models/generated/dtos/CommercialFishingApplicationEditDTO';
import { RequestProperties } from '@app/shared/services/request-properties';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { CommercialFishingPermitRegisterDTO } from '@app/models/generated/dtos/CommercialFishingPermitRegisterDTO';
import { CommercialFishingRegisterFilters } from '@app/models/generated/filters/CommercialFishingRegisterFilters';
import { SuspensionTypeNomenclatureDTO } from '@app/models/generated/dtos/SuspensionTypeNomenclatureDTO';
import { SuspensionReasonNomenclatureDTO } from '@app/models/generated/dtos/SuspensionReasonNomenclatureDTO';
import { PermitNomenclatureDTO } from '@app/models/generated/dtos/PermitNomenclatureDTO';
import { PermitLicenseForRenewalDTO } from '@app/models/generated/dtos/PermitLicenseForRenewalDTO';
import { PoundNetNomenclatureDTO } from '@app/models/generated/dtos/PoundNetNomenclatureDTO';
import { QualifiedFisherNomenclatureDTO } from '@app/models/generated/dtos/QualifiedFisherNomenclatureDTO';
import { PermitLicenseTariffCalculationParameters } from '@app/components/common-app/commercial-fishing/models/permit-license-tariff-calculation-parameters.model';
import { PaymentTariffDTO } from '@app/models/generated/dtos/PaymentTariffDTO';
import { CatchesAndSalesCommonService } from '@app/services/common-app/catches-and-sales-common.service';
import { SuspensionDataDTO } from '@app/models/generated/dtos/SuspensionDataDTO';
import { ISuspensionService } from '@app/interfaces/common-app/suspension.interface';
import { InspectedPermitLicenseNomenclatureDTO } from '@app/models/generated/dtos/InspectedPermitLicenseNomenclatureDTO';
import { FishingGearForChoiceDTO } from '@app/models/generated/dtos/FishingGearForChoiceDTO';
import { FishingGearDTO } from '@app/models/generated/dtos/FishingGearDTO';
import { PermitLicensesNomenclatureDTO } from '@app/models/generated/dtos/PermitLicensesNomenclatureDTO';
import { PermitLicenseFishingGearsApplicationDTO } from '@app/models/generated/dtos/PermitLicenseFishingGearsApplicationDTO';
import { IdentifierTypeEnum } from '@app/enums/identifier-type.enum';
import { PersonFullDataDTO } from '@app/models/generated/dtos/PersonFullDataDTO';

@Injectable({
    providedIn: 'root'
})
export class CommercialFishingPublicService extends ApplicationsRegisterPublicBaseService implements ICommercialFishingService, ISuspensionService {
    protected controller: string = 'CommercialFishingPublic';

    private readonly catchesAndSalesCommonService: CatchesAndSalesCommonService;

    public constructor(requestService: RequestService, catchesAndSalesCommonService: CatchesAndSalesCommonService) {
        super(requestService);
        this.catchesAndSalesCommonService = catchesAndSalesCommonService;
    }
    // Register
    public getAllPermits(request: GridRequestModel<CommercialFishingRegisterFilters>): Observable<GridResultModel<CommercialFishingPermitRegisterDTO>> {
        throw new Error('This method should not be called from the public app.');
    }

    public getRecord(id: number, pageCode: PageCodeEnum): Observable<CommercialFishingEditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getRegisterByApplicationId(applicationId: number, pageCode?: PageCodeEnum): Observable<CommercialFishingEditDTO> {
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
        }

        return this.requestService.get<CommercialFishingEditDTO>(this.area, this.controller, serviceMethod, {
            httpParams: params,
            responseTypeCtr: CommercialFishingEditDTO
        }).pipe(map((entry: CommercialFishingEditDTO) => {
            for (const logBook of entry.logBooks ?? []) {
                for (const shipPage of logBook.shipPagesAndDeclarations ?? []) {
                    shipPage.statusName = this.catchesAndSalesCommonService.getPageStatusTranslation(shipPage.status!);
                }

                for (const admissionPage of logBook.admissionPagesAndDeclarations ?? []) {
                    admissionPage.statusName = this.catchesAndSalesCommonService.getPageStatusTranslation(admissionPage.status!);
                }

                for (const transportationPage of logBook.transportationPagesAndDeclarations ?? []) {
                    transportationPage.statusName = this.catchesAndSalesCommonService.getPageStatusTranslation(transportationPage.status!);
                }
            }

            return entry;
        }));
    }

    public addPermit(permit: CommercialFishingEditDTO, pageCode: PageCodeEnum, ignoreLogBookConflicts: boolean): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public addAndDownloadRegister(model: CommercialFishingEditDTO, pageCode: PageCodeEnum, ignoreLogBookConflicts: boolean): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    public downloadRegister(id: number, pageCode: PageCodeEnum): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    public editPermit(permit: CommercialFishingEditDTO, pageCode: PageCodeEnum, ignoreLogBookConflicts: boolean): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public editAndDownloadRegister(model: CommercialFishingEditDTO, pageCode: PageCodeEnum, ignoreLogBookConflicts: boolean): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
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
        throw new Error('This method should not be called from the public app.');
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
        throw new Error('This method should not be called from the public app.');
    }

    // suspensions

    public getSuspensions(recordId: number, pageCode: PageCodeEnum): Observable<SuspensionDataDTO[]> {
        throw new Error('This method should not be called from the public app.');
    }

    public addSuspension(suspension: SuspensionDataDTO, id: number, pageCode: PageCodeEnum): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public addEditSuspensions(recordId: number, suspensions: SuspensionDataDTO[], pageCode: PageCodeEnum): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public deletePermit(id: number, pageCode: PageCodeEnum): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public undoDeletePermit(id: number, pageCode: PageCodeEnum): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public deleteLogBookPermitLicense(logBookPermitLicenseId: number): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public undoDeleteLogBookPermitLicense(logBookPermitLicenseId: number): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public getPermitLicenseSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getLogBookAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getPermitSuspensionAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getPermitLicenseSuspensionAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public downloadPermitFluxXml(permit: CommercialFishingEditDTO): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    // Fishing gears
    public getFishingGearAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    // Applications
    public getApplication(id: number, getRegiXData: boolean, pageCode: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams()
            .append('id', id.toString());
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
        const params: HttpParams = new HttpParams().append('permitNumber', permitNumber!.toString()).append('pageCode', pageCode.toString());
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
        throw new Error('This method should not be called from the public app.');
    };

    public getPermitLicenseApplicationDataFromPermitNumber(permitNumber: string, applicationId: number): Observable<CommercialFishingApplicationEditDTO> {
        const params: HttpParams = new HttpParams().append('permitNumber', permitNumber.toString()).append('applicationId', applicationId.toString());
        return this.requestService.get(this.area, this.controller, 'GetPermitLicenseApplicationDataFromPermit', {
            httpParams: params,
            responseTypeCtr: CommercialFishingApplicationEditDTO
        });
    };

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

    public editApplication(application: IApplicationRegister, pageCode: PageCodeEnum): Observable<number> {
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
        }

        return this.requestService.post(this.area, this.controller, serviceMethod, application, {
            properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: false })
        });
    }

    public getPermitLicenseFromFishingGearsApplication(applicationId: number): Observable<CommercialFishingEditDTO> {
        throw new Error('This method should not be called from the public app');
    }

    public completePermitLicenseFishingGearsApplication(permitLicense: CommercialFishingEditDTO): Observable<void> {
        throw new Error('This method should not be called from the public app');
    }

    public getFishingGearsByPermitLicenseRegistrationNumber(permitLicenseNumber: string, shipId: number): Observable<FishingGearDTO[]> {
        const params: HttpParams = new HttpParams().append('permitLicenseNumber', permitLicenseNumber!.toString()).append('shipId', shipId.toString());

        return this.requestService.get(this.area, this.controller, 'GetCommercialFishingPermitLicenseFishingGears', {
            httpParams: params,
            responseTypeCtr: FishingGearDTO
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
        throw new Error('This method should not be called from the public app');
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

    public getPermitLicensesNomenclatures(shipId: number): Observable<PermitLicensesNomenclatureDTO[]> {
        throw new Error('This method should not be called from the public app');
    }

    public getShipFishingGearsFromInspection(inspectionId: number): Observable<FishingGearForChoiceDTO[]> {
        throw new Error('This method should not be called from the public app');
    }

    public getFishingGearsForIds(gearIds: number[]): Observable<FishingGearDTO[]> {
        throw new Error('This method should not be called from the public app');
    }

    public getShipPermitLicensesFromInspection(shipId: number): Observable<InspectedPermitLicenseNomenclatureDTO[]> {
        throw new Error('This method should not be called from the public app');
    }

    public getPermitNomenclatures(shipId: number, showPastPermits: boolean, onlyPoundNet: boolean): Observable<PermitNomenclatureDTO[]> {
        throw new Error('This method should not be called from the public app');
    }

    public getPermitLicenseFishingGears(permitLicenseId: number): Observable<FishingGearDTO[]> {
        throw new Error('This method should not be called from the public app');
    }

    public tryGetQualifiedFisher(identifierType: IdentifierTypeEnum, identifier: string): Observable<PersonFullDataDTO | undefined> {
        throw new Error('This method should not be called from the public app');
    }
}