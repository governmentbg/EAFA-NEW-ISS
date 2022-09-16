import { Observable } from "rxjs";
import { GridRequestModel } from "@app/models/common/grid-request.model";
import { GridResultModel } from "@app/models/common/grid-result.model";
import { NomenclatureDTO } from "@app/models/generated/dtos/GenericNomenclatureDTO";
import { SystemLogDTO } from "@app/models/generated/dtos/SystemLogDTO";
import { SystemLogViewDTO } from "@app/models/generated/dtos/SystemLogViewDTO";
import { SystemLogFilters } from "@app/models/generated/filters/SystemLogFilters";

export interface ISystemLogService {
    getAll(request: GridRequestModel<SystemLogFilters>): Observable<GridResultModel<SystemLogDTO>>;
    get(id: number): Observable<SystemLogViewDTO>;
    getActionTypeCategories(): Observable<NomenclatureDTO<number>[]>;
}