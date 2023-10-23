import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map, switchMap } from 'rxjs/operators';

import { FluxFvmsDomainsEnum } from '@app/enums/flux-fvms-domains.enum';
import { IFluxVmsRequestsService } from '@app/interfaces/administration-app/flux-vms-requests.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { FluxFlapRequestDTO } from '@app/models/generated/dtos/FluxFlapRequestDTO';
import { FluxFlapRequestEditDTO } from '@app/models/generated/dtos/FluxFlapRequestEditDTO';
import { FLUXVMSRequestDTO } from '@app/models/generated/dtos/FLUXVMSRequestDTO';
import { FLUXVMSRequestEditDTO } from '@app/models/generated/dtos/FLUXVMSRequestEditDTO';
import { FluxFlapRequestFilters } from '@app/models/generated/filters/FluxFlapRequestFilters';
import { FLUXVMSRequestFilters } from '@app/models/generated/filters/FLUXVMSRequestFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestProperties } from '@app/shared/services/request-properties';
import { RequestService } from '@app/shared/services/request.service';
import { FuseTranslationLoaderService } from '@fuse/services/translation-loader.service';
import { FluxFAQueryRequestEditDTO } from '@app/models/generated/dtos/FluxFAQueryRequestEditDTO';
import { FluxSalesQueryRequestEditDTO } from '@app/models/generated/dtos/FluxSalesQueryRequestEditDTO';
import { FluxVesselQueryRequestEditDTO } from '@app/models/generated/dtos/FluxVesselQueryRequestEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { BaseAuditService } from '../common-app/base-audit.service';
import { FluxAcdrRequestDTO } from '@app/models/generated/dtos/FluxAcdrRequestDTO';
import { FluxAcdrRequestEditDTO } from '@app/models/generated/dtos/FluxAcdrRequestEditDTO';
import { FileInfoDTO } from '@app/models/generated/dtos/FileInfoDTO';
import { FluxAcdrReportDTO } from '@app/models/generated/dtos/FluxAcdrReportDTO';
import { FluxAcdrRequestFilters } from '@app/models/generated/filters/FluxAcdrRequestFilters';
import { FluxAcdrReportStatusEnum } from '@app/enums/flux-acdr-report-status.enum';


@Injectable({
    providedIn: 'root'
})
export class FluxVmsRequestsService extends BaseAuditService implements IFluxVmsRequestsService {
    protected controller: string = 'FLUXVMSRequests';

    private translateService: FuseTranslationLoaderService;

    public constructor(requestService: RequestService, translateService: FuseTranslationLoaderService) {
        super(requestService, AreaTypes.Administrative);
        this.translateService = translateService;
    }

    public getAll(request: GridRequestModel<FLUXVMSRequestFilters>): Observable<GridResultModel<FLUXVMSRequestDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAll', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public get(id: number): Observable<FLUXVMSRequestEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get', {
            httpParams: params,
            responseTypeCtr: FLUXVMSRequestEditDTO
        });
    }

    public getAllFlapRequests(request: GridRequestModel<FluxFlapRequestFilters>): Observable<GridResultModel<FluxFlapRequestDTO>> {
        return this.requestService.post(this.area, this.controller, 'GetAllFlapRequests', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        });
    }

    public getFlapRequest(id: number): Observable<FluxFlapRequestEditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetFlapRequest', {
            httpParams: params,
            responseTypeCtr: FluxFlapRequestEditDTO
        });
    }

    public addFlapRequest(flap: FluxFlapRequestEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'AddFlapRequest', flap);
    }

    public addVesselQueryRequest(flap: FluxVesselQueryRequestEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'AddVesselQueryRequest', flap);
    }

    public addSalesQueryRequest(flap: FluxSalesQueryRequestEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'AddSalesQueryRequest', flap);
    }

    public addFAQueryRequest(flap: FluxFAQueryRequestEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'AddFAQueryRequest', flap);
    }

    public addAcdrQueryRequest(acdr: FluxAcdrRequestEditDTO): Observable<void> {
        return this.requestService.post(this.area, this.controller, 'AddAcdrQueryRequest', acdr);
    }

    public importAcdrQueryRequest(id: number, fileInfo: FileInfoDTO): Observable<void> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.post(this.area, this.controller, 'ImportAcdrQueryRequest', fileInfo, {
            httpParams: params,
            properties: new RequestProperties({ asFormData: true })
        });
    }

    public getFlapRequestAudit(id: number): Observable<SimpleAuditDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'GetFlapRequestAudit', {
            httpParams: params,
            responseTypeCtr: SimpleAuditDTO
        });
    }

    public getAgreementTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get<NomenclatureDTO<number>[]>(this.area, this.controller, 'GetAgreementTypes', {
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((types: NomenclatureDTO<number>[]) => {
            for (const type of types) {
                type.displayName = `${type.code} - ${type.displayName}`;
            }

            return types;
        }));
    }

    public getTerritories(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get<NomenclatureDTO<number>[]>(this.area, this.controller, 'GetTerritories', {
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((types: NomenclatureDTO<number>[]) => {
            for (const type of types) {
                type.displayName = `${type.code} - ${type.displayName}`;
            }

            return types;
        }));
    }

    public getCoastalParties(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get<NomenclatureDTO<number>[]>(this.area, this.controller, 'GetCoastalParties', {
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((types: NomenclatureDTO<number>[]) => {
            for (const type of types) {
                type.displayName = `${type.code} - ${type.displayName}`;
            }

            return types;
        }));
    }

    public getRequestPurposes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get<NomenclatureDTO<number>[]>(this.area, this.controller, 'GetRequestPurposes', {
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((types: NomenclatureDTO<number>[]) => {
            for (const type of types) {
                type.displayName = `${type.code} - ${type.displayName}`;
            }

            return types;
        }));
    }

    public getFishingCategories(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get<NomenclatureDTO<number>[]>(this.area, this.controller, 'GetFishingCategories', {
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((types: NomenclatureDTO<number>[]) => {
            for (const type of types) {
                type.displayName = `${type.code} - ${type.displayName}`;
            }

            return types;
        }));
    }

    public getFlapQuotaTypes(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get<NomenclatureDTO<number>[]>(this.area, this.controller, 'GetFlapQuotaTypes', {
            responseTypeCtr: NomenclatureDTO
        }).pipe(map((types: NomenclatureDTO<number>[]) => {
            for (const type of types) {
                type.displayName = `${type.code} - ${type.displayName}`;
            }

            return types;
        }));
    }

    public replayRequest(id: number, serviceType: FluxFvmsDomainsEnum, methodName?: string): Observable<void> {
        let httpParams = new HttpParams();

        httpParams = httpParams.append('id', id.toString());
        httpParams = httpParams.append('serviceType', serviceType.toString());
        if (methodName != null) {
            httpParams = httpParams.append('methodName', methodName);
        }

        return this.requestService.get(this.area, this.controller, 'ReplayRequest', {
            httpParams: httpParams,
            successMessage: this.translateService.getValue('flux-vms-requests.flux-replay-success')
        });
    }

    public getAllAcdrRequests(request: GridRequestModel<FluxAcdrRequestFilters>): Observable<GridResultModel<FluxAcdrRequestDTO>> {
        type Result = GridResultModel<FluxAcdrRequestDTO>;
        type Request = GridRequestModel<FluxAcdrRequestFilters>;

        return this.requestService.post<Result, Request>(this.area, this.controller, 'GetAllAcdrRequests', request, {
            properties: RequestProperties.NO_SPINNER,
            responseTypeCtr: GridResultModel
        }).pipe(switchMap((entries: Result) => {
            for (const entry of entries.records) {
                entry.reportStatusName = this.getAcdrReportStatusTranslation(entry.reportStatus!);
            }

            const ids: number[] = entries.records.map((acdrRequest: FluxAcdrRequestDTO) => {
                return acdrRequest.fluxRequestId!;
            });

            if (ids.length === 0) {
                return of(entries);
            }

            return this.getAcdrRequestHistoryRecords(ids, request.filters).pipe(map((historyRecords: FluxAcdrReportDTO[]) => {
                for (const record of historyRecords) {
                    record.reportStatusName = this.getAcdrReportStatusTranslation(record.reportStatus!);

                    const found = entries.records.find((entry: FluxAcdrRequestDTO) => {
                        return entry.periodMonth === (record.periodStart!.getMonth() + 1)
                            && entry.periodYear === (record.periodStart!.getFullYear());
                    });

                    if (found !== undefined) {
                        if (found.historyRecords !== undefined && found.historyRecords !== null) {
                            found.historyRecords.push(new FluxAcdrReportDTO(record));
                        }
                        else {
                            found.historyRecords = [record];
                        }
                    }
                }
                return entries;
            }));
        }));
    }

    public downloadAcdrRequestContent(id: number): Observable<boolean> {
        const params: HttpParams = new HttpParams().append('id', id.toString());

        return this.requestService.download(this.area, this.controller, 'DownloadAcdrRequestContent', '', {
            httpParams: params
        });
    }

    private getAcdrRequestHistoryRecords(ids: number[], filters: FluxAcdrRequestFilters | undefined): Observable<FluxAcdrReportDTO[]> {
        const request = new AcdrRequestHistoryRecordData({ filters: filters, ids: ids });
        return this.requestService.post(this.area, this.controller, 'GetAllAcdrHistoryRequests', request, {
            responseTypeCtr: FluxAcdrReportDTO
        });
    }

    private getAcdrReportStatusTranslation(status: FluxAcdrReportStatusEnum): string {
        switch (status) {
            case FluxAcdrReportStatusEnum.GENERATED:
                return this.translateService.getValue('flux-vms-requests.acdr-report-status-generated');
            case FluxAcdrReportStatusEnum.MANUAL:
                return this.translateService.getValue('flux-vms-requests.acdr-report-status-manual');
            case FluxAcdrReportStatusEnum.DOWNLOADED:
                return this.translateService.getValue('flux-vms-requests.acdr-report-status-downloaded');
            case FluxAcdrReportStatusEnum.UPLOADED:
                return this.translateService.getValue('flux-vms-requests.acdr-report-status-uploaded');
            case FluxAcdrReportStatusEnum.SENT:
                return this.translateService.getValue('flux-vms-requests.acdr-report-status-sent');
            default:
                throw new Error('Invalid ACDR report status: ' + status);
        }
    }
}

class AcdrRequestHistoryRecordData {
    public filters: FluxAcdrRequestFilters | undefined;
    public ids: number[] = [];

    public constructor(obj?: Partial<AcdrRequestHistoryRecordData>) {
        Object.assign(this, obj);
    }
}