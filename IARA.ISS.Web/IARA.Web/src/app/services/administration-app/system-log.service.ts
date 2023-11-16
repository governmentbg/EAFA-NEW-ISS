import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

import { ISystemLogService } from '@app/interfaces/administration-app/system-log.interface';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SystemLogDTO } from '@app/models/generated/dtos/SystemLogDTO';
import { SystemLogViewDTO } from '@app/models/generated/dtos/SystemLogViewDTO';
import { SystemLogFilters } from '@app/models/generated/filters/SystemLogFilters';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { RequestService } from '@app/shared/services/request.service';
import { BaseAuditService } from '../common-app/base-audit.service';
import { BaseSystemLogDTO } from '@app/models/generated/dtos/BaseSystemLogDTO';
import { map, switchMap } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class SystemLogService extends BaseAuditService implements ISystemLogService {
    protected controller: string = 'SystemLog';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
    }

    public get(id: number): Observable<SystemLogViewDTO> {
        const params = new HttpParams().append('id', id.toString());

        return this.requestService.get(this.area, this.controller, 'Get', {
            httpParams: params,
            responseTypeCtr: SystemLogViewDTO
        });
    }

    public getAll(request: GridRequestModel<SystemLogFilters>): Observable<GridResultModel<BaseSystemLogDTO>> {
        type Result = GridResultModel<BaseSystemLogDTO>;
        type Request = GridRequestModel<SystemLogFilters>;

        return this.requestService.post<Result, Request>(this.area, this.controller, 'GetAll', request, {
            responseTypeCtr: GridResultModel
        }).pipe((switchMap((entries: Result) => {
            const eventUids: string[] = entries.records.map((entry: BaseSystemLogDTO) => entry.eventUID!);

            if (eventUids.length === 0) {
                return of(entries);
            }

            return this.getSystemLogsForTable(eventUids, request.filters).pipe(map((systemLogs: SystemLogDTO[]) => {
                for (const log of systemLogs) {
                    const found = entries.records.find((entry: BaseSystemLogDTO) => {
                        return entry.eventUID === log.eventUID;
                    });

                    if (found !== undefined && found !== null) {
                        if (found.systemLogs !== undefined && found.systemLogs !== null) {
                            found.systemLogs.push(new SystemLogDTO(log));
                        }
                        else {
                            found.systemLogs = [log];
                        }
                    }
                }
                return entries;
            }));
        })));
    }

    public getActionTypeCategories(): Observable<NomenclatureDTO<number>[]> {
        return this.requestService.get(this.area, this.controller, 'GetActionTypeCategories', {
            responseTypeCtr: NomenclatureDTO
        });
    }

    private getSystemLogsForTable(eventUids: string[], filters: SystemLogFilters | undefined): Observable<SystemLogDTO[]> {
        const request = new SystemLogRecordData({ eventUids, filters });
        return this.requestService.post(this.area, this.controller, 'GetAllSystemLogsForTable', request, {
            responseTypeCtr: SystemLogDTO
        });
    }
}

class SystemLogRecordData {
    public filters: SystemLogFilters | undefined;
    public eventUids: string[] | undefined;

    public constructor(obj?: Partial<SystemLogRecordData>) {
        Object.assign(this, obj);
    }
}
