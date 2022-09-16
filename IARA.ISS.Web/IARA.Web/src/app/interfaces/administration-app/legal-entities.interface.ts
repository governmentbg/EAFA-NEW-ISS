import { Observable } from 'rxjs';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { LegalEntityDTO } from '@app/models/generated/dtos/LegalEntityDTO';
import { LegalEntityEditDTO } from '@app/models/generated/dtos/LegalEntityEditDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { LegalEntitiesFilters } from '@app/models/generated/filters/LegalEntitiesFilters';
import { AuthorizedPersonDTO } from '@app/models/generated/dtos/AuthorizedPersonDTO';
import { IApplicationsActionsService } from '../common-app/application-actions.interface';
import { IBaseAuditService } from '../base-audit.interface';

export interface ILegalEntitiesService extends IApplicationsActionsService, IBaseAuditService {
    getAllLegalEntities(request: GridRequestModel<LegalEntitiesFilters>): Observable<GridResultModel<LegalEntityDTO>>;
    getLegalEntity(id: number): Observable<LegalEntityEditDTO>;
    addLegalEntity(legal: LegalEntityEditDTO): Observable<void>;
    editLegalEntity(legal: LegalEntityEditDTO): Observable<void>;
    getCurrentUserAsAuthorizedPerson(): Observable<AuthorizedPersonDTO>;
    getAuthorizedPersonSimpleAudit(id: number): Observable<SimpleAuditDTO>;
}