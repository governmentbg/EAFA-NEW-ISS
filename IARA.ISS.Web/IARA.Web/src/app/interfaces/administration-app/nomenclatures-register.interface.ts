import { Observable } from 'rxjs';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclaturesFilters } from '@app/models/generated/filters/NomenclaturesFilters';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { ColumnDTO } from '@app/models/generated/dtos/ColumnDTO';
import { PermissionTypeEnum } from '@app/enums/permission-type.enum';
import { NomenclatureTableDTO } from '@app/models/generated/dtos/NomenclatureTableDTO';

export interface INomenclaturesService {
    tableId: number | undefined;

    getAll(request: GridRequestModel<NomenclaturesFilters>): Observable<GridResultModel<Record<string, unknown>>>;
    get(id: number): Observable<Record<string, unknown>>;

    getTables(): Observable<NomenclatureTableDTO[]>;
    getChildNomenclatures(): Observable<Record<string, NomenclatureDTO<number>[]>>;
    getTablePermissions(): Observable<PermissionTypeEnum[]>;
    getColumns(): Observable<ColumnDTO[]>;
    getGroups(): Observable<NomenclatureDTO<number>[]>;

    add(entity: Record<string, unknown>): Observable<number>;
    edit(entity: Record<string, unknown>): Observable<void>;
    delete(id: number): Observable<void>;
    undoDelete(id: number): Observable<void>;

    getSimpleAudit(id: number): Observable<SimpleAuditDTO>;
}