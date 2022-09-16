import { Injectable } from '@angular/core';
import { HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { RequestService } from '@app/shared/services/request.service';
import { AreaTypes } from '@app/shared/enums/area-type.enum';
import { BaseAuditService } from '../common-app/base-audit.service';
import { IFluxVmsRequestsService } from '@app/interfaces/administration-app/flux-vms-requests.interface';
import { FLUXVMSRequestFilters } from '@app/models/generated/filters/FLUXVMSRequestFilters';
import { FLUXVMSRequestDTO } from '@app/models/generated/dtos/FLUXVMSRequestDTO';
import { GridRequestModel } from '@app/models/common/grid-request.model';
import { GridResultModel } from '@app/models/common/grid-result.model';
import { RequestProperties } from '@app/shared/services/request-properties';
import { FLUXVMSRequestEditDTO } from '@app/models/generated/dtos/FLUXVMSRequestEditDTO';
import { FluxFlapRequestFilters } from '@app/models/generated/filters/FluxFlapRequestFilters';
import { FluxFlapRequestDTO } from '@app/models/generated/dtos/FluxFlapRequestDTO';
import { FluxFlapRequestEditDTO } from '@app/models/generated/dtos/FluxFlapRequestEditDTO';
import { SimpleAuditDTO } from '../../models/generated/dtos/SimpleAuditDTO';
import { NomenclatureDTO } from '../../models/generated/dtos/GenericNomenclatureDTO';
import { map } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class FluxVmsRequestsService extends BaseAuditService implements IFluxVmsRequestsService {
    protected controller: string = 'FLUXVMSRequests';

    public constructor(requestService: RequestService) {
        super(requestService, AreaTypes.Administrative);
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
}