import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { IShipQuotasService } from '@app/interfaces/administration-app/ship-quotas.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { ShipQuotaDTO } from '@app/models/generated/dtos/ShipQuotaDTO';
import { ShipQuotaEditDTO } from '@app/models/generated/dtos/ShipQuotaEditDTO';
import { ShipQuotasFilters } from '@app/models/generated/filters/ShipQuotasFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';
import { QuotaHistDTO } from '@app/models/generated/dtos/QuotaHistDTO';
import { map, switchMap } from 'rxjs/operators';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { ExcelExporterRequestModel } from '@app/shared/components/data-table/models/excel-exporter-request-model.model';

@Injectable({
    providedIn: 'root'
})
export class ShipQuotasService extends BaseAuditService implements IShipQuotasService {
    protected controller: string = 'ShipQuotas';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public add(shipQuota: ShipQuotaEditDTO): Observable<number> {
        return this.requestService.post(this.area, this.controller, 'Add', shipQuota, {
            properties: new RequestProperties({ rethrowException: true, showException: true })
        });
    }

    public getAll(request: GridRequestModel<ShipQuotasFilters>): Observable<GridResultModel<ShipQuotaDTO>> {
        type Result = GridResultModel<ShipQuotaDTO>;
        type Body = GridRequestModel<ShipQuotasFilters>;

        return this.requestService.post<Result, Body>(this.area, this.controller, 'GetAll', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridRequestModel
        }).pipe(switchMap((entries: Result) => {
            const ids: number[] = entries.records.map((quota: ShipQuotaDTO) => {
                return quota.id!;
            });

            if (ids.length === 0) {
                return of(entries);
            }

            return this.getShipQuotaHistory(ids).pipe(map((changeHistoryRecords: QuotaHistDTO[]) => {
                for (const changeHistory of changeHistoryRecords) {
                    const found = entries.records.find((entry: ShipQuotaDTO) => {
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

    public getShipQuotaHistory(ids: number[]): Observable<QuotaHistDTO[]> {
        return this.requestService.post(this.area, this.controller, 'GetChangeHistory', ids, {
            responseTypeCtr: QuotaHistDTO
        });
    }

    public get(id: number): Observable<ShipQuotaEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get', {
            httpParams: params,
            responseTypeCtr: ShipQuotaEditDTO
        });
    }

    public edit(shipQuota: ShipQuotaEditDTO): Observable<void> {
        return this.requestService.put(this.area, this.controller, 'Edit', shipQuota);
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

    public downloadShipQuotaExcel(request: ExcelExporterRequestModel<ShipQuotasFilters>): Observable<boolean> {
        return this.requestService.downloadPost(this.area, this.controller, 'DownloadShipQuotaExcel', request.filename, request, {
            properties: new RequestProperties({ showException: true, rethrowException: true }),
        });
    }

    public getShipQuotasForList(id: number): Observable<NomenclatureDTO<number>[]> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetShipQuotasForList', {
            httpParams: params,
            responseTypeCtr: NomenclatureDTO
        });
    }

    public getYearlyQuotasForList(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetYearlyQuotasForList', {
            responseTypeCtr: NomenclatureDTO
        });
    }
}