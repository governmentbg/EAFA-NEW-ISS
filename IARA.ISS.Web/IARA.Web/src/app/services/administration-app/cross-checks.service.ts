import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { CrossCheckDTO } from '@app/models/generated/dtos/CrossCheckDTO';
import { CrossCheckEditDTO } from '@app/models/generated/dtos/CrossCheckEditDTO';
import { CrossChecksFilters } from '@app/models/generated/filters/CrossChecksFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '@app/services/common-app/base-audit.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { CrossCheckResultsFilters } from '@app/models/generated/filters/CrossCheckResultsFilters';
import { CrossCheckResultDTO } from '@app/models/generated/dtos/CrossCheckResultDTO';
import { CrossCheckResolutionEditDTO } from '@app/models/generated/dtos/CrossCheckResolutionEditDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';

@Injectable({
    providedIn: 'root'
})
export class CrossChecksService extends BaseAuditService {
    protected controller: string = 'CrossChecks';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    // Cross checks
    public getAllCrossChecks(request: GridRequestModel<CrossChecksFilters>): Observable<GridResultModel<CrossCheckDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllCrossChecks', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getCrossCheck(id: number): Observable<CrossCheckEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetCrossCheck', {
            httpParams: params,
            responseTypeCtr: CrossCheckEditDTO
        });
    }

    public addCrossCheck(crossCheck: CrossCheckEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'AddCrossCheck', crossCheck);
    }

    public editCrossCheck(crossCheck: CrossCheckEditDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditCrossCheck', crossCheck);
    }

    public deleteCrossCheck(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeleteCrossCheck', { httpParams: params });
    }

    public undoDeleteCrossCheck(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeleteCrossCheck', null, { httpParams: params });
    }

    public executeCrossCheck(id: number): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'ExecuteCrossCheck', id);
    }

    public executeCrossChecks(execFrequency: string): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'ExecuteCrossChecks', null, {
            httpParams: new HttpParams().append('execFrequency', execFrequency),
            successMessage: 'succ-exec-cross-checks'
        });
    }

    // Cross check results
    public getAllCrossCheckResults(request: GridRequestModel<CrossCheckResultsFilters>): Observable<GridResultModel<CrossCheckResultDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllCrossCheckResults', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public deleteCrossCheckResult(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'DeleteCrossCheckResult', { httpParams: params });
    }

    public undoDeleteCrossCheckResult(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'UndoDeleteCrossCheckResult', null, { httpParams: params });
    }

    public assignCrossCheckResult(resultId: number, userId: number): Observable<void> {
        const params = new HttpParams()
            .append('resultId', resultId.toString())
            .append('userId', userId.toString());

        return this.requestService.patch(this.area, this.controller, 'AssignCrossCheckResult', null, { httpParams: params });
    }

    public getCrossCheckResolution(resultId: number): Observable<CrossCheckResolutionEditDTO> {
        const params = new HttpParams().append('resultId', resultId.toString());

        return this.requestService.get(this.area, this.controller, 'GetCrossCheckResolution', {
            httpParams: params,
            responseTypeCtr: CrossCheckResolutionEditDTO
        });
    }

    public editCrossCheckResolution(resolution: CrossCheckResolutionEditDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'EditCrossCheckResolution', resolution);
    }

    public getCrossCheckResultSimpleAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetCrossCheckResultSimpleAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    // Nomenclatures
    public getAllReportGroups(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllReportGroups', { responseTypeCtr: NomenclatureDTO });
    }

    public getCheckResolutionTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetCheckResolutionTypes', { responseTypeCtr: NomenclatureDTO });
    }

    public getActiveRoles(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetAllActiveRoles', { responseTypeCtr: NomenclatureDTO });
    }
}
