import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { IDuplicatesRegisterService } from '@app/interfaces/common-app/duplicates-register.interface';
import { RequestService } from '@app/shared/services/request.service';
import { ApplicationsRegisterPublicBaseService } from './applications-register-public-base.service';
import { DuplicatesRegisterEditDTO } from '@app/models/generated/dtos/DuplicatesRegisterEditDTO';
import { DuplicatesApplicationDTO } from '@app/models/generated/dtos/DuplicatesApplicationDTO';
import { RequestProperties } from '@app/shared/services/request-properties';
import { QualifiedFisherNomenclatureDTO } from '@app/models/generated/dtos/QualifiedFisherNomenclatureDTO';
import { PermitLicenseNomenclatureDTO } from '@app/models/generated/dtos/PermitLicenseNomenclatureDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

@Injectable({
    providedIn: 'root'
})
export class DuplicatesRegisterPublicService extends ApplicationsRegisterPublicBaseService implements IDuplicatesRegisterService {
    protected controller: string = 'DuplicatesRegisterPublic';

    public constructor(requestService: RequestService) {
        super(requestService);
    }

    public getApplication(id: number): Observable<IApplicationRegister> {
        const params = new HttpParams().append('applicationId', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetDuplicatesApplication', {
            httpParams: params,
            responseTypeCtr: DuplicatesApplicationDTO
        });
    }

    public getRegisterByApplicationId(applicationId: number): Observable<unknown> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetRegisterByApplicationId', {
            httpParams: params,
            responseTypeCtr: DuplicatesRegisterEditDTO
        });
    }

    public addApplication(application: IApplicationRegister): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddDuplicateApplication', application, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editApplication(application: IApplicationRegister): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'EditDuplicateApplication', application, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public getDuplicateRegister(id: number): Observable<DuplicatesRegisterEditDTO> {
        throw new Error('Method should not be called from the public app.');
    }

    public addDuplicateRegister(duplicate: DuplicatesRegisterEditDTO): Observable<number> {
        throw new Error('Method should not be called from the public app.');
    }

    public addAndDownloadDuplicateRegister(duplicate: DuplicatesRegisterEditDTO): Observable<boolean> {
        throw new Error('Method should not be called from the public app.');
    }

    public downloadDuplicate(id: number): Observable<boolean> {
        throw new Error('Method should not be called from the public app.');
    }

    public getRegisteredBuyers(): Observable<NomenclatureDTO<number>[]> {
        throw new Error('Method should not be called from the public app.');
    }

    public getPermits(): Observable<NomenclatureDTO<number>[]> {
        throw new Error('Method should not be called from the public app.');
    }

    public getPermitLicenses(): Observable<PermitLicenseNomenclatureDTO[]> {
        throw new Error('Method should not be called from the public app.');
    }

    public getQualifiedFishers(): Observable<QualifiedFisherNomenclatureDTO[]> {
        throw new Error('Method should not be called from the public app.');
    }
}