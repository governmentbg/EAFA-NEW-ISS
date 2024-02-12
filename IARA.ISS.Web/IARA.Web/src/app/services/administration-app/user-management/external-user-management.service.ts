import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IUserManagementService } from '@app/interfaces/administration-app/user-management.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { ExternalUserDTO } from '@app/models/generated/dtos/ExternalUserDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { UserDTO } from '@app/models/generated/dtos/UserDTO';
import { UserManagementFilters } from '@app/models/generated/filters/UserManagementFilters';
import { SecurityService } from '@app/services/common-app/security.service';
import { TLSnackbar } from '@app/shared/components/snackbar/tl.snackbar';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { Observable } from 'rxjs';
import { BaseUserManagementService } from './base-user-management.service';

@Injectable({
    providedIn: 'root'
})
export class ExternalUserManagementService extends BaseUserManagementService implements IUserManagementService {

    protected legalController: string = 'LegalEntitiesAdministration';

    public constructor(requestService: RequestService, securityService: SecurityService, translationService: FuseTranslationLoaderService, snackbar: TLSnackbar) {
        super(requestService, securityService, translationService, snackbar);
    }

    public getAll(request: GridRequestModel<UserManagementFilters>): Observable<GridResultModel<UserDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllExternal', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getUser(id: number): Observable<ExternalUserDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetExternalUser', {
            httpParams: params,
            responseTypeCtr: ExternalUserDTO
        });
    }

    public getActiveLegals(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.legalController, 'GetActiveLegals', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    public edit(user: ExternalUserDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditExternalUser', user, {
            successMessage: 'succ-updated-user'
        });
    }

    public changeUserStatus(userId: number): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'ChangeUserToInternal', userId, {
            successMessage: 'succ-user-made-internal'
        });
    }
}