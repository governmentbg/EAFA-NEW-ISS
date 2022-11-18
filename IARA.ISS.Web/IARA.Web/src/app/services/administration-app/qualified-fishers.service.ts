import { QualifiedFishersFilters } from '@app/models/generated/filters/QualifiedFishersFilters';
import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { IQualifiedFishersService } from '../../interfaces/administration-app/qualified-fishers.interface';
import { IApplicationRegister } from '../../interfaces/common-app/application-register.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { QualifiedFisherApplicationEditDTO } from '@app/models/generated/dtos/QualifiedFisherApplicationEditDTO';
import { QualifiedFisherDTO } from '@app/models/generated/dtos/QualifiedFisherDTO';
import { QualifiedFisherEditDTO } from '@app/models/generated/dtos/QualifiedFisherEditDTO';
import { QualifiedFisherRegixDataDTO } from '@app/models/generated/dtos/QualifiedFisherRegixDataDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { ApplicationsProcessingService } from './applications-processing.service';
import { ApplicationsRegisterAdministrativeBaseService } from './applications-register-administrative-base.service';
import { PageCodeEnum } from '@app/enums/page-code.enum';

@Injectable({
    providedIn: 'root'
})
export class QualifiedFishersService extends ApplicationsRegisterAdministrativeBaseService implements IQualifiedFishersService {
    protected controller: string = 'QualifiedFishers';

    private translate: FuseTranslationLoaderService;

    public constructor(requestService: RequestService, translate: FuseTranslationLoaderService, applicationsRegisterService: ApplicationsProcessingService) {
        super(requestService, applicationsRegisterService);

        this.translate = translate;
    }

    // register

    public getAll(request: GridRequestModel<QualifiedFishersFilters>): Observable<GridResultModel<QualifiedFisherDTO>> {
        return this.requestService.post<GridResultModel<QualifiedFisherDTO>, GridRequestModel<QualifiedFishersFilters>>(this.area, this.controller, 'GetAll', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        }).pipe(map((entries: GridResultModel<QualifiedFisherDTO>) => {
            for (const record of entries.records) {
                record.diplomaOrExamLabel = this.translate.getValue(record.diplomaOrExamLabel === 'e' ? 'qualified-fishers-page.certificate' : 'qualified-fishers-page.diploma');
            }

            return entries;
        }));
    }

    public get(id: number): Observable<QualifiedFisherEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get', {
            httpParams: params,
            responseTypeCtr: QualifiedFisherEditDTO
        });
    }

    public downloadRegister(id: number): Observable<boolean> {
        const params = new HttpParams().append('registerId', id.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadQualifiedFisher', '', { httpParams: params });
    }

    public getRegisterByApplicationId(applicationId: number): Observable<unknown> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetRegisterByApplicationId', {
            httpParams: params,
            responseTypeCtr: QualifiedFisherEditDTO
        });
    }

    public add(model: QualifiedFisherEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'Add', model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public addAndDownloadRegister(model: QualifiedFisherEditDTO): Observable<boolean> {
        return this.requestService.downloadPost(this.area, this.controller, 'AddAndDownloadRegister', '', model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public edit(model: QualifiedFisherEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'Edit', model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editAndDownloadRegister(model: QualifiedFisherDTO): Observable<boolean> {
        return this.requestService.downloadPost(this.area, this.controller, 'EditAndDownloadRegister', '', model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public delete(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'Delete', { httpParams: params });
    }

    public undoDelete(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDelete', null, { httpParams: params });
    }

    // application

    public getApplication(id: number, getRegiXData: boolean): Observable<QualifiedFisherApplicationEditDTO> {
        const params = new HttpParams()
            .append('id', id.toString())
            .append('getRegiXData', getRegiXData.toString());

        return this.requestService.get(this.area, this.controller, 'GetApplication', {
            httpParams: params,
            responseTypeCtr: QualifiedFisherApplicationEditDTO
        });
    }

    public getRegixData(id: number): Observable<RegixChecksWrapperDTO<QualifiedFisherRegixDataDTO>> {
        const params = new HttpParams().append('applicationId', id.toString());

        return this.requestService.get<RegixChecksWrapperDTO<QualifiedFisherRegixDataDTO>>(this.area, this.controller, 'GetRegixData', {
            httpParams: params,
            responseTypeCtr: RegixChecksWrapperDTO
        }).pipe(map((result: RegixChecksWrapperDTO<QualifiedFisherRegixDataDTO>) => {
            result.dialogDataModel = new QualifiedFisherRegixDataDTO(result.dialogDataModel);
            result.regiXDataModel = new QualifiedFisherRegixDataDTO(result.regiXDataModel);

            return result;
        }));
    }

    public getApplicationDataForRegister(applicationId: number): Observable<QualifiedFisherEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.requestService.get(this.area, this.controller, 'GetApplicationDataForRegister', {
            httpParams: params,
            responseTypeCtr: QualifiedFisherEditDTO
        });
    }

    public addApplication(application: IApplicationRegister): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddApplication', application, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editApplication(application: IApplicationRegister, pageCode?: PageCodeEnum, fromSaveAsDraft: boolean = false): Observable<number> {
        const params = new HttpParams().append('fromSaveAsDraft', fromSaveAsDraft.toString());

        return this.requestService.post(this.area, this.controller, 'EditApplication', application, {
            httpParams: params,
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public editApplicationDataAndStartRegixChecks(model: QualifiedFisherApplicationEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditApplicationAndStartRegixChecks', model, {
            properties: new RequestProperties({ asFormData: true })
        });
    }
}
