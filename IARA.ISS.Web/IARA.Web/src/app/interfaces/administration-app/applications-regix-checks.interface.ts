import { Observable } from 'rxjs';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { ApplicationRegixCheckRequestDTO } from '@app/models/generated/dtos/ApplicationRegixCheckRequestDTO';
import { ApplicationRegiXChecksFilters } from '@app/models/generated/filters/ApplicationRegiXChecksFilters';
import { IBaseAuditService } from '../base-audit.interface';

export interface IApplicationsRegiXChecksService extends IBaseAuditService {
    getAll(request: GridRequestModel<ApplicationRegiXChecksFilters>): Observable<GridResultModel<ApplicationRegixCheckRequestDTO>>;
    get(id: number): Observable<ApplicationRegixCheckRequestDTO>;
}