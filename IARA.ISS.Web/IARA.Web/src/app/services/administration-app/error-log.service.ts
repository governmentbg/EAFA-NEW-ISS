import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { IErrorLogService } from '../../interfaces/administration-app/error-log.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { ErrorLogDTO } from '@app/models/generated/dtos/ErrorLogDTO';
import { ErrorLogFilters } from '@app/models/generated/filters/ErrorLogFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';


@Injectable({
    providedIn: 'root'
})
export class ErrorLogService implements IErrorLogService {
    protected readonly area: AreaTypes = AreaTypes.Administrative;
    protected controller: string = 'ErrorLog';
    protected requestService: RequestService;

    public constructor(requestService: RequestService) {
        this.requestService = requestService;
    }

    public getAll(request: GridRequestModel<ErrorLogFilters>): Observable<GridResultModel<ErrorLogDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAll', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }
}