import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IScientificFishingService } from '@app/interfaces/common-app/scientific-fishing.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { ScientificFishingApplicationEditDTO } from '@app/models/generated/dtos/ScientificFishingApplicationEditDTO';
import { ScientificFishingOutingDTO } from '@app/models/generated/dtos/ScientificFishingOutingDTO';
import { ScientificFishingPermitDTO } from '@app/models/generated/dtos/ScientificFishingPermitDTO';
import { ScientificFishingPermitEditDTO } from '@app/models/generated/dtos/ScientificFishingPermitEditDTO';
import { ScientificFishingPermitRegixDataDTO } from '@app/models/generated/dtos/ScientificFishingPermitRegixDataDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { ScientificFishingFilters } from '@app/models/generated/filters/ScientificFishingFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { ScientificFishingCommonService } from '../common-app/scientific-fishing-common.service';
import { ApplicationsProcessingService } from './applications-processing.service';
import { ApplicationsRegisterAdministrativeBaseService } from './applications-register-administrative-base.service';
import { SciFiPrintTypesEnum } from '@app/enums/sci-fi-print-types.enum';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { ScientificFishingReasonNomenclatureDTO } from '@app/models/generated/dtos/ScientificFishingReasonNomenclatureDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { RegisterDTO } from '@app/models/generated/dtos/RegisterDTO';

@Injectable({
    providedIn: 'root'
})
export class ScientificFishingAdministrationService extends ApplicationsRegisterAdministrativeBaseService implements IScientificFishingService {
    protected controller: string = 'ScientificFishingAdministration';

    private commonService: ScientificFishingCommonService;

    public constructor(requestService: RequestService, commonService: ScientificFishingCommonService, applicationsProcessingService: ApplicationsProcessingService) {
        super(requestService, applicationsProcessingService);

        this.commonService = commonService;
    }

    public getAllPermits(request: GridRequestModel<ScientificFishingFilters>): Observable<GridResultModel<ScientificFishingPermitDTO>> {
        return this.commonService.getAllPermits(this.area, this.controller, request);
    }

    public getRegisterByApplicationId(applicationId: number): Observable<ScientificFishingPermitEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetRegisterByApplicationId', {
            httpParams: params,
            responseTypeCtr: ScientificFishingPermitEditDTO
        });
    }

    public getApplication(id: number, getRegiXData: boolean): Observable<ScientificFishingApplicationEditDTO> {
        return this.commonService.getPermitApplication(this.area, this.controller, id, getRegiXData);
    }

    public getPermit(id: number): Observable<ScientificFishingPermitEditDTO> {
        return this.commonService.getPermit(this.area, this.controller, id);
    }

    public getRegixData(id: number): Observable<RegixChecksWrapperDTO<ScientificFishingPermitRegixDataDTO>> {
        return this.commonService.getPermitRegixData(this.area, this.controller, id);
    }

    public getApplicationDataForRegister(applicationId: number): Observable<ScientificFishingPermitEditDTO> {
        return this.commonService.getApplicationDataForRegister(this.area, this.controller, applicationId);
    }

    public getPermitHolderPhoto(holderId: number): Observable<string> {
        const params = new HttpParams().append('holderId', holderId.toString());

        return this.requestService.get(this.area, this.controller, 'GetPermitHolderPhoto', {
            httpParams: params,
            responseType: 'text',
            properties: RequestProperties.NO_SPINNER
        });
    }

    public getShipCaptainName(shipId: number): Observable<string> {
        const params = new HttpParams().append('shipId', shipId.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipCaptainName', {
            httpParams: params,
            responseType: 'text'
        });
    }

    public getCurrentUserAsSubmittedBy(): Observable<ApplicationSubmittedByDTO> {
        throw new Error('This method should not be called from the adminsitration app');
    }

    public addApplication(application: ScientificFishingApplicationEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddPermitApplication', application, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public addPermit(permit: ScientificFishingPermitEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddPermit', permit, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public addAndDownloadRegister(permit: ScientificFishingPermitEditDTO, printType: SciFiPrintTypesEnum): Observable<boolean> {
        const params: HttpParams = new HttpParams().append('printType', printType.toString());
        const registerDto: RegisterDTO<ScientificFishingPermitEditDTO> = new RegisterDTO<ScientificFishingPermitEditDTO>({
            dto: permit
        });

        return this.requestService.downloadPost(this.area, this.controller, 'AddAndDownloadRegister', '', registerDto, {
            httpParams: params,
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editApplication(application: ScientificFishingApplicationEditDTO, pageCode?: PageCodeEnum, fromSaveAsDraft: boolean | undefined = false): Observable<number> {
        const httpParams: HttpParams = new HttpParams().append('fromSaveAsDraft', fromSaveAsDraft.toString());

        return this.requestService.post(this.area, this.controller, 'EditPermitApplication', application, {
            httpParams: httpParams,
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editPermit(permit: ScientificFishingPermitEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditPermit', permit, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editAndDownloadRegister(permit: ScientificFishingPermitEditDTO, printType: SciFiPrintTypesEnum): Observable<boolean> {
        const params: HttpParams = new HttpParams().append('printType', printType.toString());
        const registerDto: RegisterDTO<ScientificFishingPermitEditDTO> = new RegisterDTO<ScientificFishingPermitEditDTO>({
            dto: permit
        });

        return this.requestService.downloadPost(this.area, this.controller, 'EditAndDownloadRegister', '', registerDto, {
            httpParams: params,
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public deletePermit(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeletePermit', { httpParams: params });
    }

    public undoDeletePermit(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeletePermit', null, { httpParams: params });
    }

    public downloadRegister(id: number, printType: SciFiPrintTypesEnum): Observable<boolean> {
        const params: HttpParams = new HttpParams()
            .append('id', id.toString())
            .append('printType', printType.toString());

        return this.requestService.downloadPost(this.area, this.controller, 'DownloadRegister', '', undefined, { httpParams: params, });
    }

    public addOuting(outing: ScientificFishingOutingDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddOuting', outing);
    }

    public getPermitReasons(): Observable<ScientificFishingReasonNomenclatureDTO[]> {
        return this.commonService.getPermitReasons(this.area, this.controller);
    }

    public getPermitStatuses(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getPermitStatuses(this.area, this.controller);
    }

    public getPermitHolderAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPermitHolderSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getPermitOutingAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.get(this.area, this.controller, 'GetPermitOutingSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    // applications
    public editApplicationDataAndStartRegixChecks(model: ScientificFishingApplicationEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditPermitApplicationAndStartRegixChecks', model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }
}
