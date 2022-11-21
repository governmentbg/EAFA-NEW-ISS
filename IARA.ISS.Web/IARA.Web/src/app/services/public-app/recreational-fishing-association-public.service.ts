import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { IRecreationalFishingAssociationService } from '@app/interfaces/common-app/recreational-fishing-association.interface';
import { AssignedApplicationInfoDTO } from '@app/models/generated/dtos/AssignedApplicationInfoDTO';
import { FishingAssociationApplicationEditDTO } from '@app/models/generated/dtos/FishingAssociationApplicationEditDTO';
import { FishingAssociationEditDTO } from '@app/models/generated/dtos/FishingAssociationEditDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { FishingAssociationPersonDTO } from '@app/models/generated/dtos/FishingAssociationPersonDTO';
import { ApplicationSubmittedByDTO } from '@app/models/generated/dtos/ApplicationSubmittedByDTO';
import { BaseAuditService } from '../common-app/base-audit.service';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { RecreationalFishingAssociationDTO } from '@app/models/generated/dtos/RecreationalFishingAssociationDTO';
import { RecreationalFishingAssociationEditDTO } from '@app/models/generated/dtos/RecreationalFishingAssociationEditDTO';
import { RecreationalFishingPossibleAssociationLegalDTO } from '@app/models/generated/dtos/RecreationalFishingPossibleAssociationLegalDTO';
import { RecreationalFishingAssociationsFilters } from '@app/models/generated/filters/RecreationalFishingAssociationsFilters';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';

@Injectable({
    providedIn: 'root'
})
export class RecreationalFishingAssociationPublicService extends BaseAuditService implements IRecreationalFishingAssociationService {
    protected controller: string = 'RecreationalFishingAssociationsPublic';
    private applicationPublicController: string = 'ApplicationsPublic';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Public);
    }

    public getAllAssociations(request: GridRequestModel<RecreationalFishingAssociationsFilters>): Observable<GridResultModel<RecreationalFishingAssociationDTO>> {
        throw new Error('This method should not be called from the public app.');
    }

    public getAssociation(id: number): Observable<RecreationalFishingAssociationEditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getPossibleAssociationLegals(): Observable<RecreationalFishingPossibleAssociationLegalDTO[]> {
        throw new Error('This method should not be called from the public app.');
    }

    public getLegalForAssociation(id: number): Observable<RecreationalFishingAssociationEditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public addAssociation(association: RecreationalFishingAssociationEditDTO): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public editAssociation(association: RecreationalFishingAssociationEditDTO): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public deleteAssociation(id: number): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public undoDeleteAssociation(id: number): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public downloadAssociationFile(fileId: number, fileName: string): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    public getFishingAssociation(id: number): Observable<FishingAssociationEditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, { httpParams: params });
    }

    public getCurrentUserAsSubmittedBy(): Observable<ApplicationSubmittedByDTO> {
        return this.requestService.get(this.area, this.applicationPublicController, 'GetCurrentUserAsSubmittedBy', {
            responseTypeCtr: ApplicationSubmittedByDTO
        });
    }

    public getCurrentUserAsFishingAssociationPerson(): Observable<FishingAssociationPersonDTO> {
        return this.requestService.get(this.area, this.controller, 'GetCurrentUserAsFishingAssociationPerson', {
            responseTypeCtr: FishingAssociationPersonDTO
        });
    }

    public getAssociationRoleName(): Observable<string> {
        return this.requestService.get(this.area, this.controller, 'GetAssociationRoleName', {
            responseType: 'text'
        });
    }

    public getApplication(id: number, getRegiXData: boolean, pageCode?: PageCodeEnum): Observable<IApplicationRegister> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAssociationApplication', {
            httpParams: params,
            responseTypeCtr: FishingAssociationApplicationEditDTO
        });
    }

    public getRegixData(id: number, pageCode?: PageCodeEnum): Observable<RegixChecksWrapperDTO<IApplicationRegister>> {
        throw new Error('This method should not be called from the public app.');
    }

    public getApplicationDataForRegister(applicationId: number, pageCode?: PageCodeEnum): Observable<IApplicationRegister> {
        throw new Error('This method should not be called from the public app.');
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

    public editApplication(application: IApplicationRegister, pageCode: PageCodeEnum, saveAsDraft: boolean): Observable<number> {
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

    public addFishingAssociation(model: FishingAssociationEditDTO): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public editFishingAssociation(model: FishingAssociationEditDTO): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public assignApplicationViaAccessCode(accessCode: string): Observable<AssignedApplicationInfoDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public editApplicationDataAndStartRegixChecks(model: IApplicationRegister): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public getFishingAssociationSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }

    public getFishingAssociationPersonSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app.');
    }
}