import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IRolesRegisterService } from '@app/interfaces/administration-app/roles-register.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PermissionGroupDTO } from '@app/models/generated/dtos/PermissionGroupDTO';
import { RoleRegisterDTO } from '@app/models/generated/dtos/RoleRegisterDTO';
import { RoleRegisterEditDTO } from '@app/models/generated/dtos/RoleRegisterEditDTO';
import { RolesRegisterFilters } from '@app/models/generated/filters/RolesRegisterFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '@app/services/common-app/base-audit.service';
import { ReportRoleGroupDTO } from '@app/models/generated/dtos/ReportRoleGroupDTO';

@Injectable({
    providedIn: 'root'
})
export class RolesRegisterService extends BaseAuditService implements IRolesRegisterService {
    protected controller: string = 'RolesRegister';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getAllRoles(request: GridRequestModel<RolesRegisterFilters>): Observable<GridResultModel<RoleRegisterDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllRoles', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getRole(id: number): Observable<RoleRegisterEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetRole', {
            httpParams: params,
            responseTypeCtr: RoleRegisterEditDTO
        });
    }

    public addRole(role: RoleRegisterEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddRole', role);
    }

    public editRole(role: RoleRegisterEditDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditRole', role);
    }

    public deleteRole(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeleteRole', { httpParams: params });
    }

    public deleteAndReplaceRole(id: number, newRoleId: number): Observable<void> {
        const params = new HttpParams()
            .append('id', id.toString())
            .append('newRoleId', newRoleId.toString());

        return this.requestService.delete(this.area, this.controller, 'DeleteAndReplaceRole', { httpParams: params });
    }

    public undoDeleteRole(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeleteRole', null, { httpParams: params });
    }

    public getPermissionGroups(): Observable<PermissionGroupDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetPermissionGroups', {
            responseTypeCtr: PermissionGroupDTO
        });
    }

    public getReportGroups(): Observable<ReportRoleGroupDTO[]> {
        return this.requestService.get(this.area, this.controller, 'GetReportGroups', {
            responseTypeCtr: ReportRoleGroupDTO
        });
    }

    public getUsersWithRole(roleId: number): Observable<NomenclatureDTO<number>[]> {
        const params = new HttpParams().append('roleId', roleId.toString());

        return this.requestService.get(this.area, this.controller, 'GetUsersWithRole', {
            httpParams: params,
            responseTypeCtr: NomenclatureDTO
        });
    }
}
