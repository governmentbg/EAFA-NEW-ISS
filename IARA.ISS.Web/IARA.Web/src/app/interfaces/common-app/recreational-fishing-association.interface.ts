import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { FishingAssociationEditDTO } from '@app/models/generated/dtos/FishingAssociationEditDTO';
import { RecreationalFishingAssociationDTO } from '@app/models/generated/dtos/RecreationalFishingAssociationDTO';
import { RecreationalFishingAssociationEditDTO } from '@app/models/generated/dtos/RecreationalFishingAssociationEditDTO';
import { RecreationalFishingPossibleAssociationLegalDTO } from '@app/models/generated/dtos/RecreationalFishingPossibleAssociationLegalDTO';
import { RecreationalFishingAssociationsFilters } from '@app/models/generated/filters/RecreationalFishingAssociationsFilters';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { IBaseAuditService } from '../base-audit.interface';
import { IApplicationsActionsService } from './application-actions.interface';

export interface IRecreationalFishingAssociationService extends IBaseAuditService, IApplicationsActionsService {
    getAllAssociations(request: GridRequestModel<RecreationalFishingAssociationsFilters>): Observable<GridResultModel<RecreationalFishingAssociationDTO>>;
    getAssociation(id: number): Observable<RecreationalFishingAssociationEditDTO>;
    getPossibleAssociationLegals(): Observable<RecreationalFishingPossibleAssociationLegalDTO[]>;
    getLegalForAssociation(id: number): Observable<RecreationalFishingAssociationEditDTO>;
    addAssociation(association: RecreationalFishingAssociationEditDTO): Observable<number>;
    editAssociation(association: RecreationalFishingAssociationEditDTO): Observable<void>;
    deleteAssociation(id: number): Observable<void>;
    undoDeleteAssociation(id: number): Observable<void>;
    downloadAssociationFile(fileId: number, fileName: string): Observable<boolean>;

    // Applications
    getAssociationRoleName(): Observable<string>;
    getFishingAssociation(id: number): Observable<FishingAssociationEditDTO>;
    addFishingAssociation(model: FishingAssociationEditDTO): Observable<number>;
    editFishingAssociation(model: FishingAssociationEditDTO): Observable<number>;
    getFishingAssociationSimpleAudit(id: number): Observable<SimpleAuditDTO>;
    getFishingAssociationPersonSimpleAudit(id: number): Observable<SimpleAuditDTO>;
}