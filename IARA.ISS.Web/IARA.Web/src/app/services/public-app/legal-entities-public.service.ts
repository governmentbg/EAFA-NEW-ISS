import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ILegalEntitiesService } from '@app/interfaces/administration-app/legal-entities.interface';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { LegalEntityApplicationEditDTO } from '@app/models/generated/dtos/LegalEntityApplicationEditDTO';
import { LegalEntityDTO } from '@app/models/generated/dtos/LegalEntityDTO';
import { LegalEntityEditDTO } from '@app/models/generated/dtos/LegalEntityEditDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { LegalEntitiesFilters } from '@app/models/generated/filters/LegalEntitiesFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { AuthorizedPersonDTO } from '@app/models/generated/dtos/AuthorizedPersonDTO';
import { ApplicationsRegisterPublicBaseService } from './applications-register-public-base.service';

@Injectable({
    providedIn: 'root'
})
export class LegalEntitiesPublicService extends ApplicationsRegisterPublicBaseService implements ILegalEntitiesService {
    protected controller: string = 'LegalEntitiesPublic';

    public constructor(requestService: RequestService) {
        super(requestService);
    }

    // register
    public getAllLegalEntities(request: GridRequestModel<LegalEntitiesFilters>): Observable<GridResultModel<LegalEntityDTO>> {
        throw new Error('This method should not be called from the public app');
    }

    public getRegisterByApplicationId(applicationId: number): Observable<LegalEntityEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        return this.requestService.get(this.area, this.controller, 'GetRegisterByApplicationId', {
            httpParams: params,
            responseTypeCtr: LegalEntityEditDTO
        });
    }

    public getLegalEntity(id: number): Observable<LegalEntityEditDTO> {
        throw new Error('This method should not be called from the public app');
    }

    public getRegixData(id: number): Observable<RegixChecksWrapperDTO<IApplicationRegister>> {
        throw new Error('This method should not be called from the public app');
    }

    public getApplicationDataForRegister(applicationId: number): Observable<IApplicationRegister> {
        throw new Error('This method should not be called from the public app');
    }

    public addLegalEntity(legal: LegalEntityEditDTO): Observable<void> {
        throw new Error('This method should not be called from the public app');
    }

    public editLegalEntity(legal: LegalEntityEditDTO): Observable<void> {
        throw new Error('This method should not be called from the public app');
    }

    public getAuthorizedPersonSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        throw new Error('This method should not be called from the public app');
    }

    public getCurrentUserAsAuthorizedPerson(): Observable<AuthorizedPersonDTO> {
        return this.requestService.get(this.area, this.controller, 'GetCurrentUserAsAuthorizedPerson', {
            responseTypeCtr: AuthorizedPersonDTO
        });
    }

    // applications
    public getApplication(id: number): Observable<LegalEntityApplicationEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetLegalEntityApplication', {
            httpParams: params,
            responseTypeCtr: LegalEntityApplicationEditDTO
        });
    }

    public addApplication(application: LegalEntityApplicationEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddLegalEntityApplication', application, {
            properties: new RequestProperties({
                showException: false,
                rethrowException: true,
                asFormData: true
            })
        });
    }

    public editApplication(application: LegalEntityApplicationEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'EditLegalEntityApplication', application, {
            properties: new RequestProperties({
                showException: false,
                rethrowException: true,
                asFormData: true
            })
        });
    }
}