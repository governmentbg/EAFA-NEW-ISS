import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { IUserManagementService } from '@app/interfaces/administration-app/user-management.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { InternalUserDTO } from '@app/models/generated/dtos/InternalUserDTO';
import { MobileDeviceDTO } from '@app/models/generated/dtos/MobileDeviceDTO';
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
export class InternalUserManagementService extends BaseUserManagementService implements IUserManagementService {
    protected controller: string = 'UserManagement';

    public constructor(requestService: RequestService, securityService: SecurityService, translationService: FuseTranslationLoaderService, snackbar: TLSnackbar) {
        super(requestService, securityService, translationService, snackbar);
    }

    public getAll(request: GridRequestModel<UserManagementFilters>): Observable<GridResultModel<UserDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllInternal', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getUser(id: number): Observable<InternalUserDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetInternalUser', {
            httpParams: params,
            responseTypeCtr: InternalUserDTO
        });
    }

    public getUserMobileDevices(id: number): Observable<MobileDeviceDTO[]> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetUserMobileDevices', {
            httpParams: params,
            responseTypeCtr: MobileDeviceDTO
        });
    }

    public add(user: InternalUserDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'AddInternalUser', user, {
            successMessage: 'succ-added-user'
        });
    }

    public addOrEditMobileDevices(userId: number, devices: MobileDeviceDTO[]): Observable<void> {
        const params = new HttpParams().append('userId', userId.toString());

        return this.requestService.post(this.area, this.controller, 'AddOrEditMobileDevices', devices, {
            httpParams: params,
            successMessage: 'succ-updated-user-devices'
        });
    }

    public edit(user: InternalUserDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditInternalUser', user, {
            successMessage: 'succ-updated-user'
        });
    }

    public reloadAllMobileDevicesAppDatabase(): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'ReloadAllMobileDevicesAppDatabase', {
            successMessage: 'succ-updated-user-mobile-devices'
        });
    }
}