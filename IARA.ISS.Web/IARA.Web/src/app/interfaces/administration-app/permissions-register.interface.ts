import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PermissionRegisterDTO } from '@app/models/generated/dtos/PermissionRegisterDTO';
import { PermissionRegisterEditDTO } from '@app/models/generated/dtos/PermissionRegisterEditDTO';
import { PermissionsRegisterFilters } from '@app/models/generated/filters/PermissionsRegisterFilters';
import { IBaseAuditService } from '@app/interfaces/base-audit.interface';

export interface IPermissionsRegisterService extends IBaseAuditService {
    getAllPermissions(request: GridRequestModel<PermissionsRegisterFilters>): Observable<GridResultModel<PermissionRegisterDTO>>;

    getPermission(id: number): Observable<PermissionRegisterEditDTO>;
    editPermission(permission: PermissionRegisterEditDTO): Observable<void>;

    getPermissionGroups(): Observable<NomenclatureDTO<number>[]>;
    getPermissionTypes(): Observable<NomenclatureDTO<number>[]>;
    getAllRoles(): Observable<NomenclatureDTO<number>[]>;
}