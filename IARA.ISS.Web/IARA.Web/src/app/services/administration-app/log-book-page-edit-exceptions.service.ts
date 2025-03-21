import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ILogBookPageEditExceptionsService } from '@app/interfaces/administration-app/log-book-page-edit-exceptions.interface';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '@app/services/common-app/base-audit.service';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { LogBookPageEditExceptionEditDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionEditDTO';
import { LogBookPageEditExceptionRegisterDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionRegisterDTO';
import { LogBookPageEditExceptionFilters } from '@app/models/generated/filters/LogBookPageEditExceptionFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { SystemUserNomenclatureDTO } from '@app/models/generated/dtos/SystemUserNomenclatureDTO';
import { LogBookPageEditNomenclatureDTO } from '@app/models/generated/dtos/LogBookPageEditNomenclatureDTO';
import { LogBookPageExceptionGroupedDTO } from '@app/models/generated/dtos/LogBookPageExceptionGroupedDTO';

@Injectable({
    providedIn: 'root'
})
export class LogBookPageEditExceptionsService extends BaseAuditService implements ILogBookPageEditExceptionsService {
    protected controller: string = 'LogBookPageEditExceptions';

    public constructor(
        requestService: RequestService
    ) {
        super(requestService, AreaTypes.Administrative);
    }

    public getAllLogBookPageEditExceptions(request: GridRequestModel<LogBookPageEditExceptionFilters>): Observable<GridResultModel<LogBookPageEditExceptionRegisterDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllLogBookPageEditExceptions', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getLogBookPageEditException(id: number): Observable<LogBookPageEditExceptionEditDTO> {
        const params: HttpParams = new HttpParams().append('id', id.toString());
        return this.requestService.get(this.area, this.controller, 'GetLogBookPageEditException', {
            httpParams: params,
            responseTypeCtr: LogBookPageEditExceptionEditDTO
        });
    }

    public addLogBookPageEditException(model: LogBookPageEditExceptionEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'AddLogBookPageEditException', model, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-add-log-book-page-edit-exception'
        });
    }

    public editLogBookPageEditException(model: LogBookPageEditExceptionEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditLogBookPageEditException', model, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-edit-log-book-page-edit-exception'
        });
    }

    public deleteLogBookPageEditException(id: number): Observable<void> {
        const params: HttpParams = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeleteLogBookPageEditException', {
            httpParams: params,
            successMessage: 'succ-delete-log-book-page-edit-exception'
        });
    }

    public restoreLogBookPageEditException(id: number): Observable<void> {
        const params: HttpParams = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'RestoreLogBookPageEditException', undefined, {
            httpParams: params,
            successMessage: 'succ-restore-log-book-page-edit-exception'
        })
    }

    public downloadFile(fileId: number): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', '', { httpParams: params });
    }

    //LogBookPageExceptionsGrouped

    public getAllGroupedLogBookPageExceptions(request: GridRequestModel<LogBookPageEditExceptionFilters>): Observable<GridResultModel<LogBookPageExceptionGroupedDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllGroupedLogBookPageExceptions', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: LogBookPageExceptionGroupedDTO
        });
    }

    public getLogBookPagesExceptionsGrouped(logBookPageExceptionIds: number[]): Observable<LogBookPageEditExceptionEditDTO> {
        return this.requestService.post(this.area, this.controller, 'GetLogBookPageExceptionsGrouped', logBookPageExceptionIds, {
            responseTypeCtr: LogBookPageEditExceptionEditDTO
        });
    }

    public addLogBookPageExceptionsGrouped(model: LogBookPageEditExceptionEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'AddLogBookPageExceptionsGrouped', model, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-add-log-book-page-exceptions'
        });
    }

    public editLogBookPageExceptionsGrouped(model: LogBookPageEditExceptionEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'EditLogBookPageExceptionsGrouped', model, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-edit-log-book-page-exceptions'
        });
    }

    public removeLogBookPageExceptionsGrouped(logBookPageExceptionIds: number[]): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'RemoveLogBookPageExceptionsGrouped', logBookPageExceptionIds, {
            successMessage: 'succ-delete-log-book-page-edit-exception'
        });
    }

    public restoreLogBookPageExceptionsGrouped(logBookPageExceptionIds: number[]): Observable<void> {
        return this.requestService.patch(this.area, this.controller, 'RestoreLogBookPageExceptionsGrouped', logBookPageExceptionIds, {
            successMessage: 'succ-restore-log-book-page-edit-exception'
        })
    }

    // Nomenclatures

    public getAllUsersNomenclature(): Observable<SystemUserNomenclatureDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllUsersNomenclature', {
            responseTypeCtr: SystemUserNomenclatureDTO
        });
    }

    public getActiveLogBooksNomenclature(logBookPageEditExceptionId?: number): Observable<LogBookPageEditNomenclatureDTO[]> {
        let params: HttpParams | undefined = undefined;

        if (logBookPageEditExceptionId !== null && logBookPageEditExceptionId !== undefined) {
            params = new HttpParams().append('logBookPageEditExceptionId', logBookPageEditExceptionId.toString());
        }

        return this.requestService.get(this.area, this.controller, 'GetActiveLogBooksNomenclature', {
            httpParams: params,
            responseTypeCtr: LogBookPageEditNomenclatureDTO
        });
    }
}