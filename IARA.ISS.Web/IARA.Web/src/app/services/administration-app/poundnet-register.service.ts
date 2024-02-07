import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IPoundnetRegisterService } from '../../interfaces/administration-app/poundnet-register.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PoundNetDTO } from '@app/models/generated/dtos/PoundNetDTO';
import { PoundnetRegisterDTO } from '@app/models/generated/dtos/PoundnetRegisterDTO';
import { PoundNetRegisterFilters } from '@app/models/generated/filters/PoundNetRegisterFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';

@Injectable({
    providedIn: 'root'
})
export class PoundnetRegisterService extends BaseAuditService implements IPoundnetRegisterService {
    protected controller: string = 'PoundNetRegister';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getAll(request: GridRequestModel<PoundNetRegisterFilters>): Observable<GridResultModel<PoundNetDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAll', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public get(id: number): Observable<PoundnetRegisterDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get', {
            httpParams: params,
            responseTypeCtr: PoundnetRegisterDTO
        });
    }

    public add(request: PoundnetRegisterDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'Add', request);
    }

    public edit(request: PoundnetRegisterDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'Edit', request);
    }

    public delete(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'Delete', { httpParams: params });
    }

    public undoDelete(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDelete', null, { httpParams: params });
    }

    public getCategories(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetCategories', { responseTypeCtr: NomenclatureDTO });
    }

    public getSeasonalTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetSeasonalTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getPoundnetStatuses(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPoundnetStatuses', { responseTypeCtr: NomenclatureDTO });
    }
}
