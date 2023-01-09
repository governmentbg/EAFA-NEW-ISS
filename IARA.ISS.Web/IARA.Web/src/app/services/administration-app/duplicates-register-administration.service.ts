import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { IApplicationRegister } from '@app/interfaces/common-app/application-register.interface';
import { IDuplicatesRegisterService } from '@app/interfaces/common-app/duplicates-register.interface';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { RequestService } from '@app/shared/services/request.service';
import { ApplicationsProcessingService } from './applications-processing.service';
import { ApplicationsRegisterAdministrativeBaseService } from './applications-register-administrative-base.service';
import { DuplicatesApplicationDTO } from '@app/models/generated/dtos/DuplicatesApplicationDTO';
import { DuplicatesApplicationRegixDataDTO } from '@app/models/generated/dtos/DuplicatesApplicationRegixDataDTO';
import { DuplicatesRegisterEditDTO } from '@app/models/generated/dtos/DuplicatesRegisterEditDTO';
import { RequestProperties } from '@app/shared/services/request-properties';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PermitLicenseNomenclatureDTO } from '@app/models/generated/dtos/PermitLicenseNomenclatureDTO';
import { QualifiedFisherNomenclatureDTO } from '@app/models/generated/dtos/QualifiedFisherNomenclatureDTO';
import { PageCodeEnum } from '@app/enums/page-code.enum';
import { RegisterDTO } from '@app/models/generated/dtos/RegisterDTO';

@Injectable({
    providedIn: 'root'
})
export class DuplicatesRegisterAdministrationService extends ApplicationsRegisterAdministrativeBaseService implements IDuplicatesRegisterService {
    protected controller: string = 'DuplicatesRegisterAdministration';

    public constructor(requestService: RequestService, applicationProcessingService: ApplicationsProcessingService) {
        super(requestService, applicationProcessingService);
    }

    public getApplication(id: number, getRegiXData: boolean): Observable<IApplicationRegister> {
        const params = new HttpParams()
            .append('applicationId', id.toString())
            .append('getRegiXData', getRegiXData.toString());

        return this.requestService.get(this.area, this.controller, 'GetDuplicatesApplication', {
            httpParams: params,
            responseTypeCtr: DuplicatesApplicationDTO
        });
    }

    public getRegixData(id: number): Observable<RegixChecksWrapperDTO<IApplicationRegister>> {
        const params = new HttpParams().append('applicationId', id.toString());

        return this.requestService.get<RegixChecksWrapperDTO<DuplicatesApplicationRegixDataDTO>>(this.area, this.controller, 'GetDuplicatesRegixData', {
            httpParams: params,
            responseTypeCtr: RegixChecksWrapperDTO
        }).pipe(map((result: RegixChecksWrapperDTO<DuplicatesApplicationRegixDataDTO>) => {
            result.dialogDataModel = new DuplicatesApplicationRegixDataDTO(result.dialogDataModel);
            result.regiXDataModel = new DuplicatesApplicationRegixDataDTO(result.regiXDataModel);

            return result;
        }));
    }

    public getApplicationDataForRegister(applicationId: number): Observable<IApplicationRegister> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetApplicationDataForRegister', {
            httpParams: params,
            responseTypeCtr: DuplicatesRegisterEditDTO
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

    public editApplication(application: IApplicationRegister, pageCode?: PageCodeEnum, saveAsDraft: boolean = false): Observable<number> {
        const params = new HttpParams().append('saveAsDraft', saveAsDraft.toString());

        return this.requestService.post(this.area, this.controller, 'EditDuplicateApplication', application, {
            httpParams: params,
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editApplicationDataAndStartRegixChecks(model: IApplicationRegister): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditDuplicatesApplicationAndStartRegixChecks', model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public getDuplicateRegister(id: number): Observable<DuplicatesRegisterEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetDuplicateRegister', {
            httpParams: params,
            responseTypeCtr: DuplicatesRegisterEditDTO
        });
    }

    public addDuplicateRegister(duplicate: DuplicatesRegisterEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddDuplicateRegister', duplicate, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public addAndDownloadDuplicateRegister(duplicate: DuplicatesRegisterEditDTO): Observable<boolean> {
        const registerDto: RegisterDTO<DuplicatesRegisterEditDTO> = new RegisterDTO<DuplicatesRegisterEditDTO>({
            dto: duplicate
        });

        return this.requestService.downloadPost(this.area, this.controller, 'AddAndDownloadDuplicateRegister', 'duplicate', registerDto, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public downloadDuplicate(id: number): Observable<boolean> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.downloadPost(this.area, this.controller, 'DownloadDuplicateRegister', 'duplicate', undefined, {
            httpParams: params
        });
    }

    public getRegisteredBuyers(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get<NomenclatureDTO<number>[]>(this.area, this.controller, 'GetRegisteredBuyers', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getPermits(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPermits', { responseTypeCtr: NomenclatureDTO });
    }

    public getPermitLicenses(): Observable<PermitLicenseNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetPermitLicenses', { responseTypeCtr: PermitLicenseNomenclatureDTO });
    }

    public getQualifiedFishers(): Observable<QualifiedFisherNomenclatureDTO[]> {
        return this.requestService.get<QualifiedFisherNomenclatureDTO[]>(this.area, this.controller, 'GetQualifiedFishers', {
            responseTypeCtr: QualifiedFisherNomenclatureDTO
        }).pipe(map((fishers: QualifiedFisherNomenclatureDTO[]) => {
            for (const fisher of fishers) {
                fisher.displayName = `${fisher.displayName} (${fisher.registrationNumber})`;
            }

            return fishers;
        }));
    }
}