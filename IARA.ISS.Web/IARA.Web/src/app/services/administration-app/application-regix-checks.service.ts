import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { ApplicationRegixCheckRequestDTO } from '@app/models/generated/dtos/ApplicationRegixCheckRequestDTO';
import { ApplicationRegiXChecksFilters } from '@app/models/generated/filters/ApplicationRegiXChecksFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';
import { IApplicationsRegiXChecksService } from '@app/interfaces/administration-app/applications-regix-checks.interface';
import { ApplicationRegixCheckRequestEditDTO } from '@app/models/generated/dtos/ApplicationRegixCheckRequestEditDTO';

@Injectable({
    providedIn: 'root'
})
export class ApplicationRegixChecksService extends BaseAuditService implements IApplicationsRegiXChecksService {
    protected controller: string = 'ApplicationsRegiXChecks';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getAll(request: GridRequestModel<ApplicationRegiXChecksFilters>): Observable<GridResultModel<ApplicationRegixCheckRequestDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAll', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public get(id: number): Observable<ApplicationRegixCheckRequestEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get', {
            httpParams: params,
            responseTypeCtr: ApplicationRegixCheckRequestEditDTO
        });
    }
}