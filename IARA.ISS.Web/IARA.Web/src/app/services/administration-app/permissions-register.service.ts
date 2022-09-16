import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IPermissionsRegisterService } from '@app/interfaces/administration-app/permissions-register.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PermissionRegisterDTO } from '@app/models/generated/dtos/PermissionRegisterDTO';
import { PermissionRegisterEditDTO } from '@app/models/generated/dtos/PermissionRegisterEditDTO';
import { PermissionsRegisterFilters } from '@app/models/generated/filters/PermissionsRegisterFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '@app/services/common-app/base-audit.service';

@Injectable({
    providedIn: 'root'
})
export class PermissionsRegisterService extends BaseAuditService implements IPermissionsRegisterService {
    protected controller: string = 'PermissionsRegister';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getAllPermissions(request: GridRequestModel<PermissionsRegisterFilters>): Observable<GridResultModel<PermissionRegisterDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllPermissions', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getPermission(id: number): Observable<PermissionRegisterEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetPermission', {
            httpParams: params,
            responseTypeCtr: PermissionRegisterEditDTO
        });
    }

    public editPermission(permission: PermissionRegisterEditDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditPermission', permission);
    }

    public getPermissionTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllPermissionTypes', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getPermissionGroups(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllPermissionGroups', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getAllRoles(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllRoles', {
            responseTypeCtr: NomenclatureDTO
        });
    }
}
