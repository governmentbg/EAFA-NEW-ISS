import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IRolesService } from '../../interfaces/administration-app/roles.interface';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';

@Injectable({
    providedIn: 'root'
})
export class RolesService extends BaseAuditService implements IRolesService {
    protected controller: string = 'Role';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getAllActiveRoles(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllActiveRoles', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getPublicActiveRoles(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetPublicActiveRoles', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getInternalActiveRoles(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetInternalActiveRoles', {
            responseTypeCtr: NomenclatureDTO
        });
    }
}
