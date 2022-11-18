import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { QualifiedFisherApplicationEditDTO } from '@app/models/generated/dtos/QualifiedFisherApplicationEditDTO';
import { ApplicationsRegisterPublicBaseService } from './applications-register-public-base.service';
import { IQualifiedFishersService } from '@app/interfaces/administration-app/qualified-fishers.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { QualifiedFisherDTO } from '@app/models/generated/dtos/QualifiedFisherDTO';
import { QualifiedFisherEditDTO } from '@app/models/generated/dtos/QualifiedFisherEditDTO';
import { QualifiedFishersFilters } from '@app/models/generated/filters/QualifiedFishersFilters';

@Injectable({
    providedIn: 'root'
})
export class QualifiedFishersPublicService extends ApplicationsRegisterPublicBaseService implements IQualifiedFishersService {
    protected controller: string = 'QualifiedFishersPublic';

    public constructor(requestService: RequestService) {
        super(requestService);
    }

    // Register
    public getAll(request: GridRequestModel<QualifiedFishersFilters>): Observable<GridResultModel<QualifiedFisherDTO>> {
        throw new Error('This method should not be called from the public app.');
    }

    public get(id: number): Observable<QualifiedFisherEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get', {
            httpParams: params,
            responseTypeCtr: QualifiedFisherEditDTO,
            properties: RequestProperties.NO_SPINNER
        });
    }

    public downloadRegister(id: number): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    public getRegisterByApplicationId(applicationId: number): Observable<QualifiedFisherEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetRegisterByApplicationId', {
            httpParams: params,
            responseTypeCtr: QualifiedFisherEditDTO
        });
    }

    public add(model: QualifiedFisherEditDTO): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public addAndDownloadRegister(model: QualifiedFisherEditDTO): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    public edit(model: QualifiedFisherEditDTO): Observable<number> {
        throw new Error('This method should not be called from the public app.');
    }

    public editAndDownloadRegister(model: QualifiedFisherDTO): Observable<boolean> {
        throw new Error('This method should not be called from the public app.');
    }

    public delete(id: number): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    public undoDelete(id: number): Observable<void> {
        throw new Error('This method should not be called from the public app.');
    }

    // Applications
    public getApplication(id: number, getRegiXData: boolean): Observable<IApplicationRegister> {
        const params = new HttpParams()
            .append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetApplication', {
            httpParams: params,
            responseTypeCtr: QualifiedFisherApplicationEditDTO
        });
    }

    public addApplication(application: IApplicationRegister): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddApplication', application, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editApplication(application: IApplicationRegister): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'EditApplication', application, {
            properties: new RequestProperties({ asFormData: true })
        });
    }
}