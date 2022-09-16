import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PoundNetDTO } from '@app/models/generated/dtos/PoundNetDTO';
import { PoundNetRegisterFilters } from '@app/models/generated/filters/PoundNetRegisterFilters';
import { IBaseAuditService } from '../base-audit.interface';
import { PoundnetRegisterDTO } from '../../models/generated/dtos/PoundnetRegisterDTO';

export interface IPoundnetRegisterService extends IBaseAuditService {
    getAll(request: GridRequestModel<PoundNetRegisterFilters>): Observable<GridResultModel<PoundNetDTO>>;
    get(id: number): Observable<PoundnetRegisterDTO>;

    add(request: PoundnetRegisterDTO): Observable<number>;
    edit(request: PoundnetRegisterDTO): Observable<void>;
    delete(id: number): Observable<void>;
    undoDelete(id: number): Observable<void>;

    getSeasonalTypes(): Observable<NomenclatureDTO<number>[]>;
    getCategories(): Observable<NomenclatureDTO<number>[]>;
    getPoundnetStatuses(): Observable<NomenclatureDTO<number>[]>;
}