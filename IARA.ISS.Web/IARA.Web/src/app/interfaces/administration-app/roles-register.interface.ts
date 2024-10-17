import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { PermissionGroupDTO } from '@app/models/generated/dtos/PermissionGroupDTO';
import { RoleRegisterDTO } from '@app/models/generated/dtos/RoleRegisterDTO';
import { RoleRegisterEditDTO } from '@app/models/generated/dtos/RoleRegisterEditDTO';
import { RolesRegisterFilters } from '@app/models/generated/filters/RolesRegisterFilters';
import { IBaseAuditService } from '@app/interfaces/base-audit.interface';
import { ReportRoleGroupDTO } from '@app/models/generated/dtos/ReportRoleGroupDTO';

export interface IRolesRegisterService extends IBaseAuditService {
    getAllRoles(request: GridRequestModel<RolesRegisterFilters>): Observable<GridResultModel<RoleRegisterDTO>>;
    getRole(id: number): Observable<RoleRegisterEditDTO>;

    addRole(role: RoleRegisterEditDTO): Observable<number>;
    editRole(role: RoleRegisterEditDTO): Observable<void>;
    deleteRole(id: number): Observable<void>;
    undoDeleteRole(id: number): Observable<void>;

    getPermissionGroups(): Observable<PermissionGroupDTO[]>;
    getReportGroups(): Observable<ReportRoleGroupDTO[]>;
}
