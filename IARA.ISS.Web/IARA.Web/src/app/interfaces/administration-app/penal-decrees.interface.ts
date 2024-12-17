import { Observable } from 'rxjs';

import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { PenalDecreeDTO } from '@app/models/generated/dtos/PenalDecreeDTO';
import { PenalDecreeEditDTO } from '@app/models/generated/dtos/PenalDecreeEditDTO';
import { PenalDecreesFilters } from '@app/models/generated/filters/PenalDecreesFilters';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PenalDecreeAuanDataDTO } from '@app/models/generated/dtos/PenalDecreeAuanDataDTO';
import { IBaseAuditService } from '@app/interfaces/base-audit.interface';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { AuanConfiscationActionsNomenclatureDTO } from '@app/models/generated/dtos/AuanConfiscationActionsNomenclatureDTO';
import { InspDeliveryTypesNomenclatureDTO } from '@app/models/generated/dtos/InspDeliveryTypesNomenclatureDTO';
import { PenalDecreeStatusEditDTO } from '@app/models/generated/dtos/PenalDecreeStatusEditDTO';
import { InspectorUserNomenclatureDTO } from '@app/models/generated/dtos/InspectorUserNomenclatureDTO';
import { AuanStatusEnum } from '@app/enums/auan-status.enum';

export interface IPenalDecreesService extends IBaseAuditService {
    getAllPenalDecrees(request: GridRequestModel<PenalDecreesFilters>): Observable<GridResultModel<PenalDecreeDTO>>;

    getPenalDecree(id: number): Observable<PenalDecreeEditDTO>;
    addPenalDecree(decree: PenalDecreeEditDTO): Observable<number>;
    editPenalDecree(decree: PenalDecreeEditDTO): Observable<void>;
    deletePenalDecree(id: number): Observable<void>;
    undoDeletePenalDecree(id: number): Observable<void>;
    downloadPenalDecree(decreeId: number): Observable<boolean>;

    getPenalDecreeAuanData(auanId: number): Observable<PenalDecreeAuanDataDTO>;
    downloadFile(fileId: number, fileName: string): Observable<boolean>;
    getAllAuans(): Observable<NomenclatureDTO<number>[]>;
    getPenalDecreeStatusTypes(): Observable<NomenclatureDTO<number>[]>;
    getPenalDecreeAuthorityTypes(): Observable<NomenclatureDTO<number>[]>;
    getCourts(): Observable<NomenclatureDTO<number>[]>;
    getPenalDecreeTypes(): Observable<NomenclatureDTO<number>[]>;
    getPenalDecreeSanctionTypes(): Observable<NomenclatureDTO<number>[]>;
    getConfiscationInstitutions(): Observable<NomenclatureDTO<number>[]>;

    getAuanDeliveryTypes(): Observable<InspDeliveryTypesNomenclatureDTO[]>;
    getAuanDeliveryConfirmationTypes(): Observable<InspDeliveryTypesNomenclatureDTO[]>;
    getConfiscationActions(): Observable<AuanConfiscationActionsNomenclatureDTO[]>;
    getConfiscatedAppliances(): Observable<NomenclatureDTO<number>[]>;
    getTurbotSizeGroups(): Observable<NomenclatureDTO<number>[]>;
    getInspectorUsernames(): Observable<InspectorUserNomenclatureDTO[]>;
    getPenalDecreeStatusAudit(id: number): Observable<SimpleAuditDTO>;

    addPenalDecreeStatus(status: PenalDecreeStatusEditDTO): Observable<number>;
    editPenalDecreeStatus(status: PenalDecreeStatusEditDTO): Observable<void>;
    deletePenalDecreeStatus(id: number): Observable<void>;
    undoDeletePenalDecreeStatus(id: number): Observable<void>;
    updateDecreeStatus(decreeId: number, status: AuanStatusEnum): Observable<void>;
}