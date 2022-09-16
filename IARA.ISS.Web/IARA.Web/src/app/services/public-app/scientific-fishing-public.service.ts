import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IApplicationRegister } from '../../interfaces/common-app/application-register.interface';
import { IScientificFishingService } from '../../interfaces/common-app/scientific-fishing.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { AssignedApplicationInfoDTO } from '@app/models/generated/dtos/AssignedApplicationInfoDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { ScientificFishingApplicationEditDTO } from '@app/models/generated/dtos/ScientificFishingApplicationEditDTO';
import { ScientificFishingOutingDTO } from '@app/models/generated/dtos/ScientificFishingOutingDTO';
import { ScientificFishingPermitDTO } from '@app/models/generated/dtos/ScientificFishingPermitDTO';
import { ScientificFishingPermitEditDTO } from '@app/models/generated/dtos/ScientificFishingPermitEditDTO';
import { ScientificFishingPermitRegixDataDTO } from '@app/models/generated/dtos/ScientificFishingPermitRegixDataDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { ScientificFishingPublicFilters } from '@app/models/generated/filters/ScientificFishingPublicFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';
import { ScientificFishingCommonService } from '../common-app/scientific-fishing-common.service';
import { SciFiPrintTypesEnum } from '@app/enums/sci-fi-print-types.enum';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { ScientificFishingReasonNomenclatureDTO } from '@app/models/generated/dtos/ScientificFishingReasonNomenclatureDTO';

@Injectable({
    providedIn: 'root'
})
export class ScientificFishingPublicService extends BaseAuditService implements IScientificFishingService {
    protected controller: string = 'ScientificFishingPublic';

    private commonService: ScientificFishingCommonService;

    public constructor(requestService: RequestService, commonService: ScientificFishingCommonService) {
        super(requestService, AreaTypes.Public);

        this.commonService = commonService;
    }

    public getAllPermits(request: GridRequestModel<ScientificFishingPublicFilters>): Observable<GridResultModel<ScientificFishingPermitDTO>> {
        return this.commonService.getAllPermits(this.area, this.controller, request);
    }

    public getRegisterByApplicationId(applicationId: number): Observable<ScientificFishingPermitEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        return this.requestService.get(this.area, this.controller, 'GetRegisterByApplicationId', {
            httpParams: params,
            responseTypeCtr: ScientificFishingPermitEditDTO
        });
    }

    public getApplication(id: number): Observable<ScientificFishingApplicationEditDTO> {
        return this.commonService.getPermitApplication(this.area, this.controller, id);
    }

    public getRegixData(id: number): Observable<RegixChecksWrapperDTO<ScientificFishingPermitRegixDataDTO>> {
        throw new Error('This method should not be called from the public app');
    }

    public getApplicationDataForRegister(applicationId: number): Observable<ScientificFishingPermitEditDTO> {
        throw new Error('This method should not be called from the public app');
    }

    public getPermit(id: number): Observable<ScientificFishingPermitEditDTO> {
        return this.commonService.getPermit(this.area, this.controller, id);
    }

    public getCurrentUserAsSubmittedBy(): Observable<ApplicationSubmittedByDTO> {
        return this.requestService.get(this.area, this.controller, 'GetCurrentUserAsSubmittedBy', {
            responseTypeCtr: ApplicationSubmittedByDTO
        });
    }

    public getPermitHolderPhoto(holderId: number): Observable<string> {
        const params = new HttpParams().append('holderId', holderId.toString());

        return this.requestService.get(this.area, this.controller, 'GetPermitHolderPhoto', {
            httpParams: params,
            responseType: 'text',
            properties: RequestProperties.NO_SPINNER
        });
    }

    public addApplication(application: ScientificFishingApplicationEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddPermitApplication', application, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public addPermit(permit: ScientificFishingPermitEditDTO): Observable<number> {
        throw new Error('This method should not be called from the public app');
    }

    public addAndDownloadRegister(permit: ScientificFishingPermitEditDTO, printType: SciFiPrintTypesEnum): Observable<boolean> {
        throw new Error('This method should not be called from the public app');
    }

    public editApplication(application: ScientificFishingApplicationEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'EditPermitApplication', application, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editPermit(permit: ScientificFishingPermitEditDTO): Observable<void> {
        throw new Error('This method should not be called from the public app');
    }

    public editAndDownloadRegister(permit: ScientificFishingPermitEditDTO, printType: SciFiPrintTypesEnum): Observable<boolean> {
        throw new Error('This method should not be called from the public app');
    }

    public deletePermit(id: number): Observable<void> {
        throw new Error('This method should not be called from the public app');
    }

    public undoDeletePermit(id: number): Observable<void> {
        throw new Error('This method should not be called from the public app');
    }

    public downloadRegister(id: number, printType: SciFiPrintTypesEnum): Observable<boolean> {
        throw new Error('This method should not be called from the public app');
    }

    public addOuting(outing: ScientificFishingOutingDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddOuting', outing);
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, { httpParams: params });
    }

    public getPermitReasons(): Observable<ScientificFishingReasonNomenclatureDTO[]> {
        return this.commonService.getPermitReasons(this.area, this.controller);
    }

    public getPermitStatuses(): Observable<NomenclatureDTO<number>[]> {
        return this.commonService.getPermitStatuses(this.area, this.controller);
    }

    public getPermitHolderAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app');
    }

    public getPermitOutingAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app');
    }

    // applications
    public assignApplicationViaAccessCode(accessCode: string): Observable<AssignedApplicationInfoDTO> {
        throw new Error('This method should not be called from the public app');
    }

    public editApplicationDataAndStartRegixChecks(model: IApplicationRegister): Observable<void> {
        throw new Error('This method should not be called from the public app');
    }
}
