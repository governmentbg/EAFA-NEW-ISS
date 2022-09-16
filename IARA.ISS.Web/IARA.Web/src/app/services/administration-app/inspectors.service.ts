import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IInspectorsService } from '@app/interfaces/administration-app/inspectors.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { InspectorsRegisterDTO } from '@app/models/generated/dtos/InspectorsRegisterDTO';
import { InspectorsRegisterEditDTO } from '@app/models/generated/dtos/InspectorsRegisterEditDTO';
import { UnregisteredPersonEditDTO } from '@app/models/generated/dtos/UnregisteredPersonEditDTO';
import { InspectorsFilters } from '@app/models/generated/filters/InspectorsFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';

@Injectable({
    providedIn: 'root'
})
export class InspectorsService extends BaseAuditService implements IInspectorsService {
    protected controller: string = 'Inspectors';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getAllRegistered(request: GridRequestModel<InspectorsFilters>): Observable<GridResultModel<InspectorsRegisterDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllRegistered', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getAllUnregistered(request: GridRequestModel<InspectorsFilters>): Observable<GridResultModel<InspectorsRegisterDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllUnregistered', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public addInspector(inspectors: InspectorsRegisterEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddInspector', inspectors);
    }

    public addUnregisteredInspector(inspectors: UnregisteredPersonEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddUnregisteredInspector', inspectors);
    }

    public editInspector(inspector: InspectorsRegisterEditDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditInspector', inspector);
    }

    public editUnregisteredInspector(inspector: UnregisteredPersonEditDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditUnregisteredInspector', inspector);
    }

    public deleteInspector(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeleteInspector', { httpParams: params });
    }

    public undoDeleteInspector(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeleteInspector', null, { httpParams: params });
    }
}