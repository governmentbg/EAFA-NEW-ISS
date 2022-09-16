import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ITranslationManagementService } from '@app/interfaces/administration-app/translation-management.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { TranslationManagementDTO } from '@app/models/generated/dtos/TranslationManagementDTO';
import { TranslationManagementFilters } from '@app/models/generated/filters/TranslationManagementFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';
import { TranslationManagementEditDTO } from '@app/models/generated/dtos/TranslationManagementEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';

@Injectable({
    providedIn: 'root'
})
export class TranslationManagementService extends BaseAuditService implements ITranslationManagementService {
    protected controller: string = 'TranslationsManagement';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getAllLabelTranslations(request: GridRequestModel<TranslationManagementFilters>): Observable<GridResultModel<TranslationManagementDTO>> {
        const params = new HttpParams().append('helper', false.toString());

        return this.requestService.post(this.area, this.controller, 'GetAll', request, {
            httpParams: params,
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getAllHelpTranslations(request: GridRequestModel<TranslationManagementFilters>): Observable<GridResultModel<TranslationManagementDTO>> {
        const params = new HttpParams().append('helper', true.toString());

        return this.requestService.post(this.area, this.controller, 'GetAll', request, {
            httpParams: params,
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public get(id: number): Observable<TranslationManagementEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get', {
            httpParams: params,
            responseTypeCtr: TranslationManagementEditDTO
        });
    }

    public getByKey(key: string): Observable<TranslationManagementEditDTO> {
        const params = new HttpParams().append('key', key.toString());

        return this.requestService.get(this.area, this.controller, 'GetByKey', {
            httpParams: params,
            responseTypeCtr: TranslationManagementEditDTO
        });
    }

    public add(request: TranslationManagementEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'Add', request);
    }

    public edit(request: TranslationManagementEditDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'Edit', request);
    }

    public getGroups(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetGroups', { responseTypeCtr: NomenclatureDTO });
    }
}