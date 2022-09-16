import { Observable } from 'rxjs';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { PatrolVehiclesDTO } from '@app/models/generated/dtos/PatrolVehiclesDTO';
import { PatrolVehiclesEditDTO } from '@app/models/generated/dtos/PatrolVehiclesEditDTO';
import { PatrolVehiclesFilters } from '@app/models/generated/filters/PatrolVehiclesFilters';

export interface IPatrolVehiclesService {
    getAll(request: GridRequestModel<PatrolVehiclesFilters>): Observable<GridResultModel<PatrolVehiclesDTO>>;
    deletePatrolVehicle(id: number): Observable<void>;
    undoDeletePatrolVehicle(id: number): Observable<void>;
    addPatrolVehicle(patrolVehicle: PatrolVehiclesEditDTO): Observable<number>;
    editPatrolVehicle(patrolVehicle: PatrolVehiclesEditDTO): Observable<void>;
    getPatrolVehicle(id: number): Observable<PatrolVehiclesDTO>;
}