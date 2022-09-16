import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MyProfileDTO } from '@app/models/generated/dtos/MyProfileDTO';
import { UserPasswordDTO } from '@app/models/generated/dtos/UserPasswordDTO';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';

@Injectable({
    providedIn: 'root'
})
export class MyProfileCommonService {
    private requestService: RequestService;

    public constructor(requestService: RequestService) {
        this.requestService = requestService;
    }

    public getUserProfile(area: AreaTypes, controller: string, id: number): Observable<MyProfileDTO> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.get(area, controller, 'GetUserProfile', {
            httpParams: params,
            responseTypeCtr: MyProfileDTO
        });
    }

    public getUserPhoto(area: AreaTypes, controller: string, id: number): Observable<string> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.get(area, controller, 'GetUserPhoto', {
            httpParams: params,
            responseType: 'text',
            properties: RequestProperties.NO_SPINNER
        });
    }

    public updateUserProfile(area: AreaTypes, controller: string, profileData: MyProfileDTO): Observable<void> {
        return this.requestService.post(area, controller, 'UpdateUserProfile', profileData, {
            properties: new RequestProperties({ asFormData: true }),
            successMessage: 'succ-updated-user-profile'
        });
    }

    public changePassword(area: AreaTypes, controller: string, userPassword: UserPasswordDTO): Observable<void> {
        return this.requestService.post(area, controller, 'ChangeUserPassword', userPassword, {
            properties: new RequestProperties({
                showException: false,
                rethrowException: true
            }),
            successMessage: 'succ-updated-password'
        });
    }
}