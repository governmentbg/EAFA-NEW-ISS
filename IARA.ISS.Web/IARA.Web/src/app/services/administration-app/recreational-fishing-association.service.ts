import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { RecreationalFishingAssociationDTO } from '@app/models/generated/dtos/RecreationalFishingAssociationDTO';
import { RecreationalFishingAssociationEditDTO } from '@app/models/generated/dtos/RecreationalFishingAssociationEditDTO';
import { RecreationalFishingPossibleAssociationLegalDTO } from '@app/models/generated/dtos/RecreationalFishingPossibleAssociationLegalDTO';
import { RecreationalFishingAssociationsFilters } from '@app/models/generated/filters/RecreationalFishingAssociationsFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';
import { IRecreationalFishingAssociationService } from '@app/interfaces/common-app/recreational-fishing-association.interface';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { AssignedApplicationInfoDTO } from '@app/models/generated/dtos/AssignedApplicationInfoDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { FishingAssociationEditDTO } from '@app/models/generated/dtos/FishingAssociationEditDTO';
import { FishingAssociationApplicationEditDTO } from '@app/models/generated/dtos/FishingAssociationApplicationEditDTO';
import { FishingAssociationRegixDataDTO } from '@app/models/generated/dtos/FishingAssociationRegixDataDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';

@Injectable({
    providedIn: 'root'
})
export class RecreationalFishingAssociationService extends BaseAuditService implements IRecreationalFishingAssociationService {
    protected controller: string = 'RecreationalFishingAssociations';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getAllAssociations(request: GridRequestModel<RecreationalFishingAssociationsFilters>): Observable<GridResultModel<RecreationalFishingAssociationDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllAssociations', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getAssociation(id: number): Observable<RecreationalFishingAssociationEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAssociation', {
            httpParams: params,
            responseTypeCtr: RecreationalFishingAssociationEditDTO
        });
    }

    public getPossibleAssociationLegals(): Observable<RecreationalFishingPossibleAssociationLegalDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetPossibleAssociationLegals', {
            responseTypeCtr: RecreationalFishingPossibleAssociationLegalDTO
        });
    }

    public getLegalForAssociation(id: number): Observable<RecreationalFishingAssociationEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetLegalForAssociation', {
            httpParams: params,
            responseTypeCtr: RecreationalFishingAssociationEditDTO
        });
    }

    public addAssociation(association: RecreationalFishingAssociationEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddAssociation', association, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editAssociation(association: RecreationalFishingAssociationEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditAssociation', association, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public deleteAssociation(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeleteAssociation', { httpParams: params });
    }

    public undoDeleteAssociation(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeleteAssociation', null, { httpParams: params });
    }

    public downloadAssociationFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadAssociationFile', fileName, { httpParams: params });
    }

    // Applications
    public getAssociationRoleName(): Observable<string> {
        return this.requestService.get(this.area, this.controller, 'GetAssociationRoleName', {
            responseType: 'text'
        });
    }

    public getFishingAssociation(id: number): Observable<FishingAssociationEditDTO> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.get(this.area, this.controller, 'GetFishingAssociation', {
            httpParams: params,
            responseTypeCtr: FishingAssociationEditDTO
        });
    }

    public getApplication(id: number, getRegiXData: boolean, pageCode?: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams()
            .append('id', id.toString())
            .append('getRegiXData', getRegiXData.toString());

        return this.requestService.get(this.area, this.controller, 'GetAssociationApplication', {
            httpParams: params,
            responseTypeCtr: FishingAssociationApplicationEditDTO
        });
    }

    public getRegixData(applicationId: number, pageCode?: PageCodeEnum): Observable<RegixChecksWrapperDTO<IApplicationRegister>> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get<RegixChecksWrapperDTO<FishingAssociationRegixDataDTO>>(this.area, this.controller, 'GetAssociationRegixData', {
            httpParams: params,
            responseTypeCtr: RegixChecksWrapperDTO
        }).pipe(map((result: RegixChecksWrapperDTO<FishingAssociationRegixDataDTO>) => {
            result.dialogDataModel = new FishingAssociationRegixDataDTO(result.dialogDataModel);
            result.regiXDataModel = new FishingAssociationRegixDataDTO(result.regiXDataModel);

            return result;
        }));
    }

    public getApplicationDataForRegister(applicationId: number, pageCode?: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        return this.requestService.get(this.area, this.controller, 'GetApplicationDataForRegister', {
            httpParams: params,
            responseTypeCtr: FishingAssociationEditDTO
        });
    }

    public getRegisterByApplicationId(applicationId: number, pageCode?: PageCodeEnum): Observable<unknown> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        return this.requestService.get(this.area, this.controller, 'GetRegisterByApplicationId', {
            httpParams: params,
            responseTypeCtr: FishingAssociationEditDTO
        });
    }

    public addApplication(application: IApplicationRegister, pageCode?: PageCodeEnum): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddAssociationApplication', application, {
            properties: new RequestProperties({
                showException: false,
                rethrowException: true,
                asFormData: true
            })
        });
    }

    public editApplication(application: IApplicationRegister, pageCode: PageCodeEnum, saveAsDraft: boolean = false): Observable<number> {
        const params = new HttpParams().append('saveAsDraft', saveAsDraft.toString());
        return this.requestService.post(this.area, this.controller, 'EditAssociationApplication', application, {
            httpParams: params,
            properties: new RequestProperties({
                showException: false,
                rethrowException: true,
                asFormData: true
            })
        });
    }

    public editApplicationDataAndStartRegixChecks(model: IApplicationRegister): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditAssociationApplicationAndStartRegixChecks', model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public addFishingAssociation(model: FishingAssociationEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddFishingAssociation', model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editFishingAssociation(model: FishingAssociationEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'EditFishingAssociation', model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, { httpParams: params });
    }

    public assignApplicationViaAccessCode(accessCode: string): Observable<AssignedApplicationInfoDTO> {
        const params = new HttpParams().append('accessCode', accessCode);
        return this.requestService.post(this.area, this.controller, 'AssignApplicationViaAccessCode', null, { httpParams: params });
    }

    public getFishingAssociationSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.get(this.area, this.controller, 'GetFishingAssociationSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getFishingAssociationPersonSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.get(this.area, this.controller, 'GetFishingAssociationPersonSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }
}
