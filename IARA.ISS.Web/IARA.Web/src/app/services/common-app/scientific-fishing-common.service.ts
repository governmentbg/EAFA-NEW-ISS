import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { RegixChecksWrapperDTO } from '@app/models/generated/dtos/RegixChecksWrapperDTO';
import { ScientificFishingApplicationEditDTO } from '@app/models/generated/dtos/ScientificFishingApplicationEditDTO';
import { ScientificFishingPermitDTO } from '@app/models/generated/dtos/ScientificFishingPermitDTO';
import { ScientificFishingPermitEditDTO } from '@app/models/generated/dtos/ScientificFishingPermitEditDTO';
import { ScientificFishingPermitHolderDTO } from '@app/models/generated/dtos/ScientificFishingPermitHolderDTO';
import { ScientificFishingPermitRegixDataDTO } from '@app/models/generated/dtos/ScientificFishingPermitRegixDataDTO';
import { ScientificFishingFilters } from '@app/models/generated/filters/ScientificFishingFilters';
import { ScientificFishingPublicFilters } from '@app/models/generated/filters/ScientificFishingPublicFilters';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { ScientificFishingReasonNomenclatureDTO } from '@app/models/generated/dtos/ScientificFishingReasonNomenclatureDTO';

type FiltersUnion = ScientificFishingPublicFilters | ScientificFishingFilters;

@Injectable({
    providedIn: 'root'
})
export class ScientificFishingCommonService {
    private http: RequestService;

    public constructor(requestService: RequestService) {
        this.http = requestService;
    }

    public getAllPermits(area: AreaTypes, controller: string, request: GridRequestModel<FiltersUnion>): Observable<GridResultModel<ScientificFishingPermitDTO>> {
        return this.http.post(area, controller, 'GetAllPermits', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getPermitHolders(area: AreaTypes, controller: string, permitIDs: number[]): Observable<ScientificFishingPermitHolderDTO[]> {
        return this.http.post(area, controller, 'GetPermitHolders', permitIDs, {
            responseTypeCtr: ScientificFishingPermitHolderDTO
        });
    }

    public getPermitApplication(area: AreaTypes, controller: string, permitId: number, getRegiXData?: boolean): Observable<ScientificFishingApplicationEditDTO> {
        let params = new HttpParams()
            .append('id', permitId.toString());

        if (getRegiXData !== null && getRegiXData !== undefined) {
            params = params.append('getRegiXData', getRegiXData.toString());
        }

        return this.http.get(area, controller, 'GetPermitApplication', {
            httpParams: params,
            responseTypeCtr: ScientificFishingApplicationEditDTO
        });
    }

    public getPermit(area: AreaTypes, controller: string, permitId: number): Observable<ScientificFishingPermitEditDTO> {
        const params = new HttpParams().append('id', permitId.toString());
        return this.http.get(area, controller, 'GetPermit', {
            httpParams: params,
            responseTypeCtr: ScientificFishingPermitEditDTO
        });
    }

    public getPermitRegixData(area: AreaTypes, controller: string, applicationId: number): Observable<RegixChecksWrapperDTO<ScientificFishingPermitRegixDataDTO>> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.http.get(area, controller, 'GetPermitRegixData', {
            httpParams: params,
            responseTypeCtr: RegixChecksWrapperDTO
        });
    }

    public getApplicationDataForRegister(area: AreaTypes, controller: string, applicationId: number): Observable<ScientificFishingPermitEditDTO> {
        const params = new HttpParams().append('applicationId', applicationId.toString());

        return this.http.get(area, controller, 'GetApplicationDataForRegister', {
            httpParams: params,
            responseTypeCtr: ScientificFishingPermitEditDTO
        });
    }

    public getPermitReasons(area: AreaTypes, controller: string): Observable<ScientificFishingReasonNomenclatureDTO[]> {
        return this.http.get(area, controller, 'GetPermitReasons', {
            responseTypeCtr: ScientificFishingReasonNomenclatureDTO
        });
    }

    public getPermitStatuses(area: AreaTypes, controller: string): Observable<NomenclatureDTO<number>[]> {
        return this.http.get(area, controller, 'GetPermitStatuses', {
            responseTypeCtr: NomenclatureDTO
        });
    }
}
