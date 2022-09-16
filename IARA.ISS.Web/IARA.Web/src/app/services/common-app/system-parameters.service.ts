import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { RequestService } from '@app/shared/services/request.service';
import { SystemPropertiesDTO } from '@app/models/generated/dtos/SystemPropertiesDTO';

@Injectable({
    providedIn: 'root'
})
export class SystemParametersService {
    public async systemParameters(): Promise<SystemPropertiesDTO> {
        if (this._systemParameters === null || this._systemParameters === undefined) {
            await this.getSystemProperties();
        }
        return this._systemParameters!;
    }

    private readonly area: AreaTypes = AreaTypes.Nomenclatures;
    private controller: string = 'Nomenclatures';
    private requestService: RequestService;
    private _systemParameters: SystemPropertiesDTO | undefined;

    public constructor(requestService: RequestService) {
        this.requestService = requestService;
    }

    private async getSystemProperties(): Promise<void> {
        this._systemParameters = (await this.requestService.get(this.area, this.controller, 'GetSystemProperties', { responseTypeCtr: SystemPropertiesDTO }).toPromise()) as SystemPropertiesDTO;
    }
}