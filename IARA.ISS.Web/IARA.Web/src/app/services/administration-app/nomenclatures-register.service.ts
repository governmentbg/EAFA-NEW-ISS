import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { PermissionTypeEnum } from '@app/enums/permission-type.enum';
import { INomenclaturesService } from '@app/interfaces/administration-app/nomenclatures-register.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { ColumnDTO } from '@app/models/generated/dtos/ColumnDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { NomenclatureTableDTO } from '@app/models/generated/dtos/NomenclatureTableDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { NomenclaturesFilters } from '@app/models/generated/filters/NomenclaturesFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { DateUtils } from '@app/shared/utils/date.utils';
import { BaseAuditService } from '../common-app/base-audit.service';

@Injectable({
    providedIn: 'root'
})
export class NomenclaturesRegisterService extends BaseAuditService implements INomenclaturesService {
    protected controller: string = 'NomenclaturesRegister';

    public tableId: number = 0;

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getAll(request: GridRequestModel<NomenclaturesFilters>): Observable<GridResultModel<Record<string, unknown>>> {
        if (request.filters === null || request.filters === undefined) {
            request.filters = new NomenclaturesFilters({ tableId: this.tableId });
        }
        else {
            request.filters.tableId = this.tableId;
        }

        return this.requestService.post(this.area, this.controller, 'GetAll', request, {
            properties: RequestProperties.DEFAULT,
            responseTypeCtr: GridResultModel
        });
    }

    public get(id: number): Observable<Record<string, unknown>> {
        const params = new HttpParams()
            .append('tableId', this.tableId.toString())
            .append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get', { httpParams: params });
    }

    public getTables(): Observable<NomenclatureTableDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetTables', {
            responseTypeCtr: NomenclatureTableDTO
        });
    }

    public getChildNomenclatures(): Observable<Record<string, NomenclatureDTO<number>[]>> {
        const params = new HttpParams().append('tableId', this.tableId.toString());
        return this.requestService.get(this.area, this.controller, 'GetChildNomenclatures', { httpParams: params });
    }

    public getTablePermissions(): Observable<PermissionTypeEnum[]> {
        const params = new HttpParams().append('tableId', this.tableId.toString());
        return this.requestService.get(this.area, this.controller, 'GetTablePermissions', { httpParams: params });
    }

    public getColumns(): Observable<ColumnDTO[]> {
        const params = new HttpParams().append('tableId', this.tableId.toString());

        return this.requestService.get(this.area, this.controller, 'GetColumns', {
            httpParams: params,
            responseTypeCtr: ColumnDTO
        });
    }

    public getGroups(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetGroups', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public add(entity: Record<string, unknown>): Observable<number> {
        entity = this.stringifyFields(entity);

        const params = new HttpParams().append('tableId', this.tableId.toString());
        return this.requestService.post(this.area, this.controller, 'Add', entity, { httpParams: params });
    }

    public edit(entity: Record<string, unknown>): Observable<void> {
        entity = this.stringifyFields(entity);

        const params = new HttpParams().append('tableId', this.tableId.toString());
        return this.requestService.post(this.area, this.controller, 'Edit', entity, { httpParams: params });
    }

    public delete(id: number): Observable<void> {
        const params = new HttpParams()
            .append('tableId', this.tableId.toString())
            .append('id', id.toString());

        return this.requestService.delete(this.area, this.controller, 'Delete', { httpParams: params });
    }

    public undoDelete(id: number): Observable<void> {
        const params = new HttpParams()
            .append('tableId', this.tableId.toString())
            .append('id', id.toString());

        return this.requestService.patch(this.area, this.controller, 'Restore', null, { httpParams: params });
    }

    public getSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams()
            .append('tableId', this.tableId.toString())
            .append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetAuditInfoForTable', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    private stringifyFields(entity: any): any {
        for (const key of Object.keys(entity)) {
            if (entity[key] instanceof Date) {
                entity[key] = DateUtils.ToDateTimeString(entity[key]);
            }
            else if (entity[key] !== null && entity[key] !== undefined && entity[key] !== '' && !Number.isNaN(entity[key])) {
                entity[key] = entity[key].toString();
            }
            else if (entity[key] === '') {
                entity[key] = null;
            }
        }

        return entity;
    }
}
