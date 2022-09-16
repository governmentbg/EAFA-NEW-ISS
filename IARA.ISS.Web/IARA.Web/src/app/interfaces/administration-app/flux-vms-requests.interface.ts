import { Observable } from 'rxjs';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { FLUXVMSRequestDTO } from '@app/models/generated/dtos/FLUXVMSRequestDTO';
import { FLUXVMSRequestFilters } from '@app/models/generated/filters/FLUXVMSRequestFilters';
import { FluxFlapRequestFilters } from '@app/models/generated/filters/FluxFlapRequestFilters';
import { FluxFlapRequestDTO } from '@app/models/generated/dtos/FluxFlapRequestDTO';
import { FluxFlapRequestEditDTO } from '@app/models/generated/dtos/FluxFlapRequestEditDTO';
import { SimpleAuditDTO } from '@app/models/generated/dtos/SimpleAuditDTO';
import { NomenclatureDTO } from '@app/models/generated/dtos/GenericNomenclatureDTO';
import { IBaseAuditService } from '../base-audit.interface';

export interface IFluxVmsRequestsService extends IBaseAuditService {
    getAll(request: GridRequestModel<FLUXVMSRequestFilters>): Observable<GridResultModel<FLUXVMSRequestDTO>>;
    get(id: number): Observable<FLUXVMSRequestDTO>;

    getAllFlapRequests(request: GridRequestModel<FluxFlapRequestFilters>): Observable<GridResultModel<FluxFlapRequestDTO>>;
    getFlapRequest(id: number): Observable<FluxFlapRequestEditDTO>;
    addFlapRequest(flap: FluxFlapRequestEditDTO): Observable<void>;
    getFlapRequestAudit(id: number): Observable<SimpleAuditDTO>;

    getAgreementTypes(): Observable<NomenclatureDTO<number>[]>;
    getCoastalParties(): Observable<NomenclatureDTO<number>[]>;
    getRequestPurposes(): Observable<NomenclatureDTO<number>[]>;
    getFishingCategories(): Observable<NomenclatureDTO<number>[]>;
    getFlapQuotaTypes(): Observable<NomenclatureDTO<number>[]>;
}