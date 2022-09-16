import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { RequestService } from '@app/shared/services/request.service';

export abstract class BaseAuditService {
    protected abstract controller: string;
    protected requestService: RequestService;
    protected area: AreaTypes;

    public constructor(requestService: RequestService, area: AreaTypes) {
        this.requestService = requestService;
        this.area = area;
    }

    public getSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAuditInfo', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }
}