import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { ISystemLogService } from '../../interfaces/administration-app/system-log.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SystemLogDTO } from '@app/models/generated/dtos/SystemLogDTO';
import { SystemLogViewDTO } from '@app/models/generated/dtos/SystemLogViewDTO';
import { SystemLogFilters } from '@app/models/generated/filters/SystemLogFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';

@Injectable({
    providedIn: 'root'
})
export class SystemLogService extends BaseAuditService implements ISystemLogService {
    protected controller: string = 'SystemLog';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public get(id: number): Observable<SystemLogViewDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get', {
            httpParams: params,
            responseTypeCtr: SystemLogViewDTO
        });
    }

    public getAll(request: GridRequestModel<SystemLogFilters>): Observable<GridResultModel<SystemLogDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAll', request, {
            responseTypeCtr: GridResultModel
        });
    }

    public getActionTypeCategories(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetActionTypeCategories', {
            responseTypeCtr: NomenclatureDTO
        });
    }
}