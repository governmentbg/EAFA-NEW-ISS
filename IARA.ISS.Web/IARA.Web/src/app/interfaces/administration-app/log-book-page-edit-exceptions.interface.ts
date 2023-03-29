import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { LogBookPageEditExceptionRegisterDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionRegisterDTO';
import { LogBookPageEditExceptionFilters } from '@app/models/generated/filters/LogBookPageEditExceptionFilters';
import { LogBookPageEditExceptionEditDTO } from '@app/models/generated/dtos/LogBookPageEditExceptionEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SystemUserNomenclatureDTO } from '@app/models/generated/dtos/SystemUserNomenclatureDTO';
import { IBaseAuditService } from '../base-audit.interface';

export interface ILogBookPageEditExceptionsService extends IBaseAuditService {
    getAllLogBookPageEditExceptions(request: GridRequestModel<LogBookPageEditExceptionFilters>): Observable<GridResultModel<LogBookPageEditExceptionRegisterDTO>>;
    getLogBookPageEditException(id: number): Observable<LogBookPageEditExceptionEditDTO>;

    addLogBookPageEditException(model: LogBookPageEditExceptionEditDTO): Observable<void>;
    editLogBookPageEditException(model: LogBookPageEditExceptionEditDTO): Observable<void>;
    deleteLogBookPageEditException(id: number): Observable<void>;
    restoreLogBookPageEditException(id: number): Observable<void>;

    getAllUsersNomenclature(): Observable<SystemUserNomenclatureDTO[]>;
    getActiveLogBooksNomenclature(logBookPageEditExceptionId?: number): Observable<NomenclatureDTO<number>[]>;
}