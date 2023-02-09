import { HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
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
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { NomenclatureDTO } from '../../models/generated/dtos/GenericNomenclatureDTO';
import { SimpleAuditDTO } from '../../models/generated/dtos/SimpleAuditDTO';
import { BaseAuditService } from '../common-app/base-audit.service';


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
}