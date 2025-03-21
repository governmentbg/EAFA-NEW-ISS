import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { LogBookPageEditExceptionRegisterDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionRegisterDTO';
import { LogBookPageEditExceptionFilters } from '@app/models/generated/filters/LogBookPageEditExceptionFilters';
import { LogBookPageEditExceptionEditDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SystemUserNomenclatureDTO } from '@app/models/generated/dtos/SystemUserNomenclatureDTO';
import { IBaseAuditService } from '../base-audit.interface';
import { LogBookPageExceptionGroupedDTO } from '@app/models/generated/dtos/LogBookPageExceptionGroupedDTO';

export interface ILogBookPageEditExceptionsService extends IBaseAuditService {
    getAllLogBookPageEditExceptions(request: GridRequestModel<LogBookPageEditExceptionFilters>): Observable<GridResultModel<LogBookPageEditExceptionRegisterDTO>>;
    getAllGroupedLogBookPageExceptions(request: GridRequestModel<LogBookPageEditExceptionFilters>): Observable<GridResultModel<LogBookPageExceptionGroupedDTO>>;
    getLogBookPageEditException(id: number): Observable<LogBookPageEditExceptionEditDTO>;

    addLogBookPageEditException(model: LogBookPageEditExceptionEditDTO): Observable<void>;
    editLogBookPageEditException(model: LogBookPageEditExceptionEditDTO): Observable<void>;
    deleteLogBookPageEditException(id: number): Observable<void>;
    restoreLogBookPageEditException(id: number): Observable<void>;

    downloadFile(fileId: number): Observable<boolean>;

    getLogBookPagesExceptionsGrouped(logBookPageExceptionIds: number[]): Observable<LogBookPageEditExceptionEditDTO>;
    addLogBookPageExceptionsGrouped(model: LogBookPageEditExceptionEditDTO): Observable<void>;
    editLogBookPageExceptionsGrouped(model: LogBookPageEditExceptionEditDTO): Observable<void>;
    removeLogBookPageExceptionsGrouped(logBookPageExceptionIds: number[]): Observable<void>;
    restoreLogBookPageExceptionsGrouped(logBookPageExceptionIds: number[]): Observable<void>;

    getAllUsersNomenclature(): Observable<SystemUserNomenclatureDTO[]>;
    getActiveLogBooksNomenclature(logBookPageEditExceptionId?: number): Observable<NomenclatureDTO<number>[]>;
}