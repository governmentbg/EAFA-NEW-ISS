import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { ILegalEntitiesService } from '@app/interfaces/administration-app/legal-entities.interface';
import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { LegalEntityDTO } from '@app/models/generated/dtos/LegalEntityDTO';
import { LegalEntitiesFilters } from '@app/models/generated/filters/LegalEntitiesFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { LegalEntityApplicationEditDTO } from '@app/models/generated/dtos/LegalEntityApplicationEditDTO';
import { ApplicationsProcessingService } from './applications-processing.service';
import { LegalEntityEditDTO } from '@app/models/generated/dtos/LegalEntityEditDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { LegalEntityRegixDataDTO } from '@app/models/generated/dtos/LegalEntityRegixDataDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { ApplicationsRegisterAdministrativeBaseService } from './applications-register-administrative-base.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { AuthorizedPersonDTO } from '@app/models/generated/dtos/AuthorizedPersonDTO';

@Injectable({
    providedIn: 'root'
})
export class LegalEntitiesAdministrationService extends ApplicationsRegisterAdministrativeBaseService implements ILegalEntitiesService {
    protected controller: string = 'LegalEntitiesAdministration';

    public constructor(requestService: RequestService, applicationsProcessingService: ApplicationsProcessingService) {
        super(requestService, applicationsProcessingService);
    }

    // register
    public getAllLegalEntities(request: GridRequestModel<LegalEntitiesFilters>): Observable<GridResultModel<LegalEntityDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllLegalEntities', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getRegisterByApplicationId(applicationId: number, pageCode: PageCodeEnum): Observable<LegalEntityEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());
        return this.requestService.get(this.area, this.controller, 'GetRegisterByApplicationId', {
            httpParams: params,
            responseTypeCtr: LegalEntityEditDTO
        });
    }

    public getLegalEntity(id: number): Observable<LegalEntityEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetLegalEntity', {
            httpParams: params,
            responseTypeCtr: LegalEntityEditDTO
        });
    }

    public getRegixData(applicationId: number): Observable<RegixChecksWrapperDTO<LegalEntityRegixDataDTO>> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get<RegixChecksWrapperDTO<LegalEntityRegixDataDTO>>(this.area, this.controller, 'GetLegalEntityRegixData', {
            httpParams: params,
            responseTypeCtr: RegixChecksWrapperDTO
        }).pipe(map((result: RegixChecksWrapperDTO<LegalEntityRegixDataDTO>) => {
            result.dialogDataModel = new LegalEntityRegixDataDTO(result.dialogDataModel);
            result.regiXDataModel = new LegalEntityRegixDataDTO(result.regiXDataModel);

            return result;
        }));
    }

    public getApplicationDataForRegister(applicationId: number): Observable<LegalEntityEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetApplicationDataForRegister', {
            httpParams: params,
            responseTypeCtr: LegalEntityEditDTO
        });
    }

    public addLegalEntity(legal: LegalEntityEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'AddLegalEntity', legal, {
            properties: new RequestProperties({
                showException: true,
                rethrowException: true,
                asFormData: true
            })
        });
    }

    public editLegalEntity(legal: LegalEntityEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditLegalEntity', legal, {
            properties: new RequestProperties({
                showException: true,
                rethrowException: true,
                asFormData: true
            })
        });
    }

    public getCurrentUserAsAuthorizedPerson(): Observable<AuthorizedPersonDTO> {
        throw new Error('This method should not be called from the administrative app.');
    }

    public getAuthorizedPersonSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAuthorizedPersonSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    // applications
    public getApplication(id: number, getRegiXData: boolean): Observable<LegalEntityApplicationEditDTO> {
        const params = new HttpParams()
            .append('id', id.toString())
            .append('getRegiXData', getRegiXData.toString());

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

    public editApplication(application: LegalEntityApplicationEditDTO, pageCode?: PageCodeEnum, saveAsDraft: boolean = false): Observable<number> {
        const params = new HttpParams().append('saveAsDraft', saveAsDraft.toString());

        return this.requestService.post(this.area, this.controller, 'EditLegalEntityApplication', application, {
            httpParams: params,
            properties: new RequestProperties({
                showException: false,
                rethrowException: true,
                asFormData: true
            })
        });
    }

    public editApplicationDataAndStartRegixChecks(model: IApplicationRegister): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditLegalEntityApplicationAndStartRegixChecks', model, {
            properties: new RequestProperties({
                showException: false,
                rethrowException: true
            })
        });
    }
}