import { Observable } from "rxjs";
import { GridRequestModel } from "@app/models/common/grid-request.model";
import { GridResultModel } from "@app/models/common/grid-result.model";
import { ErrorLogDTO } from "@app/models/generated/dtos/ErrorLogDTO";
import { ErrorLogFilters } from "@app/models/generated/filters/ErrorLogFilters";


export interface IErrorLogService {
    getAll(request: GridRequestModel<ErrorLogFilters>): Observable<GridResultModel<ErrorLogDTO>>;
}