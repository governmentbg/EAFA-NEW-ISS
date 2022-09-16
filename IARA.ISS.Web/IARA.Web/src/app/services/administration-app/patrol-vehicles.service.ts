import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IPatrolVehiclesService } from '@app/interfaces/administration-app/patrol-vehicles.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { PatrolVehiclesDTO } from '@app/models/generated/dtos/PatrolVehiclesDTO';
import { PatrolVehiclesEditDTO } from '@app/models/generated/dtos/PatrolVehiclesEditDTO';
import { PatrolVehiclesFilters } from '@app/models/generated/filters/PatrolVehiclesFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';


@Injectable({
    providedIn: 'root'
})
export class PatrolVehiclesService extends BaseAuditService implements IPatrolVehiclesService {
    protected controller: string = 'PatrolVehicles';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }
    public getAll(request: GridRequestModel<PatrolVehiclesFilters>): Observable<GridResultModel<PatrolVehiclesDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAll', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public deletePatrolVehicle(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeletePatrolVehicle', { httpParams: params });
    }

    public undoDeletePatrolVehicle(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeletePatrolVehicle', null, { httpParams: params });
    }

    public addPatrolVehicle(patrolVehicle: PatrolVehiclesEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddPatrolVehicle', patrolVehicle);
    }

    public editPatrolVehicle(patrolVehicle: PatrolVehiclesEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditPatrolVehicle', patrolVehicle);
    }

    public getPatrolVehicle(id: number): Observable<PatrolVehiclesDTO> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.get(this.area, this.controller, 'GetPatrolVehicle', {
            httpParams: params,
            responseTypeCtr: PatrolVehiclesDTO
        });
    }
}