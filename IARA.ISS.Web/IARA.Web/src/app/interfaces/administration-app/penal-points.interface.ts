import { IBaseAuditService } from '@app/interfaces/base-audit.interface';
import { Observable } from 'rxjs';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { PenalPointsDTO } from '@app/models/generated/dtos/PenalPointsDTO';
import { PenalPointsFilters } from '@app/models/generated/filters/PenalPointsFilters';
import { PenalPointsEditDTO } from '@app/models/generated/dtos/PenalPointsEditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { PenalPointsAuanDecreeDataDTO } from '@app/models/generated/dtos/PenalPointsAuanDecreeDataDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { PermitNomenclatureDTO } from '@app/models/generated/dtos/PermitNomenclatureDTO';
import { PenalPointsOrderDTO } from '@app/models/generated/dtos/PenalPointsOrderDTO';

export interface IPenalPointsService extends IBaseAuditService {
    getAllPenalPoints(request: GridRequestModel<PenalPointsFilters>): Observable<GridResultModel<PenalPointsDTO>>;

    getPenalPoints(id: number): Observable<PenalPointsEditDTO>;
    addPenalPoints(auan: PenalPointsEditDTO): Observable<number>;
    editPenalPoints(auan: PenalPointsEditDTO): Observable<void>;
    deletePenalPoints(id: number): Observable<void>;
    undoDeletePenalPoints(id: number): Observable<void>;
    getPenalPointsAuanDecreeData(decreeId: number): Observable<PenalPointsAuanDecreeDataDTO>;
    downloadFile(fileId: number, fileName: string): Observable<boolean>;

    getAllPenalDecrees(): Observable<NomenclatureDTO<number>[]>;
    getAllPenalPointsStatuses(): Observable<NomenclatureDTO<number>[]>;
    getPermitNomenclatures(shipId: number, onlyPoundNet: boolean): Observable<PermitNomenclatureDTO[]>;
    getPermitLicensesNomenclatures(shipId: number): Observable<PermitNomenclatureDTO[]>;
    getPermitOrders(ownerId: number, isFisher: boolean, isPermitOwnerPerson: boolean): Observable<PenalPointsOrderDTO[]>;

    getPenalPointsStatusAudit(id: number): Observable<SimpleAuditDTO>;
}