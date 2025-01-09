import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { FishingActivityReportDTO } from '@app/models/generated/dtos/FishingActivityReportDTO';
import { FishingActivityReportsFilters } from '@app/models/generated/filters/FishingActivityReportsFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';
import { FileInfoDTO } from '../../models/generated/dtos/FileInfoDTO';

@Injectable({
    providedIn: 'root'
})
export class FishingActivityReportsService extends BaseAuditService {
    protected controller: string = 'FishingActivityReports';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public getAllFishingActivityReports(request: GridRequestModel<FishingActivityReportsFilters>): Observable<GridResultModel<FishingActivityReportDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllFishingActivityReports', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public generateFishingActivityReportLanding(tripIdentifier: string): Observable<void> {
        const params = new HttpParams().append('tripIdentifier', tripIdentifier);
        return this.requestService.post(this.area, this.controller, 'GenerateFishingActivityReportLanding', null, { httpParams: params });
    }

    public fishingActivityReportReplayTrip(tripIdentifier: string): Observable<void> {
        const params = new HttpParams().append('tripIdentifier', tripIdentifier);
        return this.requestService.post(this.area, this.controller, 'FishingActivityReportReplayTrip', null, { httpParams: params });
    }

    public fishingActivityReportReplayMessage(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.post(this.area, this.controller, 'FishingActivityReportReplayMessage', null, { httpParams: params });
    }

    public sendFishingActivityTripToFlux(tripIdentifier: string): Observable<void> {
        const params = new HttpParams().append('tripIdentifier', tripIdentifier);
        return this.requestService.post(this.area, this.controller, 'SendFishingActivityTripToFlux', null, { httpParams: params });
    }

    public sendFishingActivityReportToFlux(id: number, onlyLast: boolean): Observable<void> {
        const params = new HttpParams()
            .append('id', id.toString())
            .append('onlyLast', onlyLast.toString());

        return this.requestService.post(this.area, this.controller, 'SendFishingActivityReportToFlux', null, { httpParams: params });
    }

    public downloadFishingActivityReport(id: number): Observable<boolean> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.downloadPost(this.area, this.controller, 'DownloadFishingActivityReport', '', undefined, {
            httpParams: params
        });
    }

    public uploadFishingActivityReport(id: number, file: FileInfoDTO): Observable<void> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.post(this.area, this.controller, 'UploadFishingActivityReport', file, {
            httpParams: params,
            properties: new RequestProperties({ asFormData: true })
        });
    }
}