import { Injectable } from '@angular/core';
import { RequestService } from '@app/shared/services/request.service';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { Observable } from 'rxjs';
import { ChangeUserDataDTO } from '@app/models/generated/dtos/ChangeUserDataDTO';
import { RequestProperties } from '@app/shared/services/request-properties';

@Injectable({
    providedIn: 'root'
})
export class UsersService {
    private readonly area: AreaTypes = AreaTypes.Common;

    private controller: string = 'User';
    private requestService: RequestService;

    public constructor(requestService: RequestService) {
        this.requestService = requestService;
    }

    public updateUserData(userData: ChangeUserDataDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'UpdateAllUserData', userData, {
            properties: RequestProperties.DEFAULT,
            successMessage: 'succ-updated-user-profile'
        });
    }

    public getUserData(): Observable<ChangeUserDataDTO> {
        return this.requestService.get(this.area, this.controller, 'GetAllUserData');
    }
}