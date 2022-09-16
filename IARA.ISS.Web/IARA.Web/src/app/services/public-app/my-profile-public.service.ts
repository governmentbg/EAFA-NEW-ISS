import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { IMyProfileService } from '../../interfaces/common-app/my-profile.interface';
import { MyProfileDTO } from '@app/models/generated/dtos/MyProfileDTO';
import { UserPasswordDTO } from '@app/models/generated/dtos/UserPasswordDTO';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';
import { MyProfileCommonService } from '../common-app/my-profile-common.service';

@Injectable({
    providedIn: 'root'
})
export class MyProfilePublicService extends BaseAuditService implements IMyProfileService {
    protected controller: string = 'MyProfilePublic';
    private commonService: MyProfileCommonService;

    public constructor(requestService: RequestService, commonService: MyProfileCommonService) {
        super(requestService, AreaTypes.Public);
        this.commonService = commonService;
    }

    public getUserProfile(id: number): Observable<MyProfileDTO> {
        return this.commonService.getUserProfile(this.area, this.controller, id);
    }

    public getUserPhoto(id: number): Observable<string> {
        return this.commonService.getUserPhoto(this.area, this.controller, id);
    }

    public updateUserProfile(profileData: MyProfileDTO): Observable<void> {
        return this.commonService.updateUserProfile(this.area, this.controller, profileData);
    }

    public changePassword(userPassword: UserPasswordDTO): Observable<void> {
        return this.commonService.changePassword(this.area, this.controller, userPassword);
    }
}