import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { IYearlyQuotasService } from '@app/interfaces/administration-app/yearly-quotas.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { QuotaHistDTO } from '@app/models/generated/dtos/QuotaHistDTO';
import { YearlyQuotaDTO } from '@app/models/generated/dtos/YearlyQuotaDTO';
import { YearlyQuotaEditDTO } from '@app/models/generated/dtos/YearlyQuotaEditDTO';
import { YearlyQuotasFilters } from '@app/models/generated/filters/YearlyQuotasFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ExcelExporterRequestModel } from '@app/shared/components/data-table/models/excel-exporter-request-model.model';

@Injectable({
    providedIn: 'root'
})
export class YearlyQuotasService extends BaseAuditService implements IYearlyQuotasService {
    protected controller: string = 'YearlyQuotas';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public add(quota: YearlyQuotaEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'Add', quota, {
            properties: new RequestProperties({ asFormData: true, rethrowException: true, showException: true })
        });
    }

    public getAll(request: GridRequestModel<YearlyQuotasFilters>): Observable<GridResultModel<YearlyQuotaDTO>> {
        type Result = GridResultModel<YearlyQuotaDTO>;
        type Body = GridRequestModel<YearlyQuotasFilters>;

        return this.requestService.post<Result, Body>(this.area, this.controller, 'GetAll', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridRequestModel
        }).pipe(switchMap((entries: Result) => {
            const ids: number[] = entries.records.map((quota: YearlyQuotaDTO) => {
                return quota.id!;
            });

            if (ids.length === 0) {
                return of(entries);
            }

            return this.getYearlyQuotaHistory(ids)
                .pipe(map((changeHistoryRecords: QuotaHistDTO[]) => {
                    for (const changeHistory of changeHistoryRecords) {
                        const found = entries.records.find((entry: YearlyQuotaDTO) => {
                            return entry.id === changeHistory.quotaId;
                        });

                        if (found !== undefined) {
                            if (found.changeHistoryRecords !== undefined && found.changeHistoryRecords !== null) {
                                found.changeHistoryRecords.push(changeHistory);
                            }
                            else {
                                found.changeHistoryRecords = [changeHistory];
                            }
                        }
                    }

                    return entries;
                }));
        }));
    }

    public getYearlyQuotaHistory(ids: number[]): Observable<QuotaHistDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetChangeHistory', ids, {
            responseTypeCtr: QuotaHistDTO
        });
    }

    public getLastYearsQuota(newQuotaId: number): Observable<YearlyQuotaEditDTO> {
        const params = new HttpParams().append('newQuotaId', newQuotaId.toString());

        return this.requestService.get(this.area, this.controller, 'GetLastYearsQuota', {
            httpParams: params
        });
    }

    public get(id: number): Observable<YearlyQuotaEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get', {
            httpParams: params,
            responseTypeCtr: YearlyQuotaEditDTO,
            properties: RequestProperties.NO_SPINNER
        });
    }

    public edit(quota: YearlyQuotaEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'Edit', quota, {
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public transfer(newQuotaId: number, oldQuotaId: number, transferValue: number, basis: string): Observable<void> {
        const params = new HttpParams()
            .append('newQuotaId', newQuotaId.toString())
            .append('oldQuotaId', oldQuotaId.toString())
            .append('transferValue', transferValue.toString())
            .append('basis', basis);

        return this.requestService.delete(this.area, this.controller, 'Transfer', {
            httpParams: params,
            successMessage: 'succ-transfered-quota'
        });
    }

    public delete(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.delete(this.area, this.controller, 'Delete', { httpParams: params });
    }

    public undoDelete(id: number): Observable<void> {
        const params = new HttpParams().append('id', id.toString());
        return this.requestService.patch(this.area, this.controller, 'Restore', null, { httpParams: params });
    }

    public downloadFile(fileId: number, fileName: string): Observable<boolean> {
        const params = new HttpParams().append('id', fileId.toString());
        return this.requestService.download(this.area, this.controller, 'DownloadFile', fileName, { httpParams: params });
    }

    public downloadYearlyQuotaExcel(request: ExcelExporterRequestModel<YearlyQuotasFilters>): Observable<boolean> {
        return this.requestService.downloadPost(this.area, this.controller, 'DownloadYearlyQuotaExcel', request.filename, request, {
            properties: new RequestProperties({ showException: true, rethrowException: true }),
        });
    }

    public getYearlyQuotasForList(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetYearlyQuotasForList', {
            responseTypeCtr: NomenclatureDTO
        });
    }
}