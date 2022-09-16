import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { ApplicationRegisterDTO } from '@app/models/generated/dtos/ApplicationRegisterDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { ApplicationsRegisterFilters } from '@app/models/generated/filters/ApplicationsRegisterFilters';
import { IApplicationsActionsService } from '../common-app/application-actions.interface';

export interface IApplicationsRegisterService extends IApplicationsActionsService {
    getAllApplications(request: GridRequestModel<ApplicationsRegisterFilters>): Observable<GridResultModel<ApplicationRegisterDTO>>;

    deleteApplication(id: number): Observable<void>;
    undoDeleteApplication(id: number): Observable<void>;

    getApplicationStatuses(): Observable<NomenclatureDTO<number>[]>;
    getApplicationTypes(): Observable<NomenclatureDTO<number>[]>;
    getApplicationSources(): Observable<NomenclatureDTO<number>[]>;

    getSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getApplicationHistorySimpleAudit(id: number): Observable<SimpleAuditDTO>;
}